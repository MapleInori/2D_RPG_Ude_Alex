using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // TODO:笔记：单例模式的各种写法，最好写的健壮一些
    // 使用单例模式确保全局只有一个AudioManager实例
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试从场景中查找
                instance = FindObjectOfType<AudioManager>();

                // 如果不存在则创建
                if (instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    instance = obj.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    // 音效最小播放距离：超出此距离的音效将被忽略（优化性能）
    [SerializeField] private float sfxMinimumDistance;
    // 音效源数组：存放所有音效的AudioSource组件
    [SerializeField] private AudioSource[] sfx;
    // 背景音乐源数组：存放所有BGM的AudioSource组件
    [SerializeField] private AudioSource[] bgm;

    // TODO：笔记：音效播放管理，有哪些方法，哪种更好呢？
    // 也可以用更详细的分组，来控制各种音效的播放，那样在Inspector引用的时候更清晰，乱了也好弄，但是这里的代码写起来应该会复杂一些
    // 或者每个声音单独给一个变量，变量名区分这是什么音效，这样在Inspector中更直观，在调用的时候传入变量名即可，
    // 这样代码也不复杂，也不用担心什么操作导致音效顺序混乱，然后导致根据索引播放的音效全部出错。
    // 只不过变量的多少完全取决于音效数量，如果音效过多，会变得非常冗长.
    // 如果将音效放在可能用到的地方，开启PlayerOnAwake，启用时自动播放，控制激活也是一种方法，不过这种方法会导致音效管理分散在各个脚本中，难以维护？

    // BGM总开关：控制背景音乐是否播放
    public bool playBgm;
    // 当前播放的BGM索引
    private int bgmIndex;

    // SFX安全锁：防止游戏初始化时立即播放音效（避免场景加载噪音）
    private bool canPlaySFX;

    private void Awake()
    {
        // 单例初始化：确保不会存在多个AudioManager实例
        if (instance != null && instance != this)
            Destroy(gameObject); // 销毁当前实例避免冲突
        else
            instance = this; // 首次创建时设置实例引用
        // 重新加载会清空音效列表，（并不是，是因为引用的音效资源都在场景中，而场景中的音效都被销毁重建了，所以导致这里列表被清空了），取消下一行即可
        //DontDestroyOnLoad(gameObject);
        // 延迟1秒启用音效：避免场景加载时立即播放音效，由于开始一瞬间会打开技能树保证引用正常和数据加载，所以这里延迟一秒，不然刚开始会有一声切换声音
        Invoke("AllowSFX", 1f);
    }

    private void Update()
    {
        // BGM状态机：根据总开关控制背景音乐播放
        if (!playBgm)
            StopAllBGM(); // 关闭所有BGM
        else
        {
            // 自动续播：当前BGM意外停止时重新播放（防止意外中断）
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }
    /// <summary>
    /// 播放音效：根据索引播放指定的音效，如果不需要距离检测，可以传入null作为源对象。
    /// </summary>
    /// <param name="_sfxIndex"></param>
    /// <param name="_source"></param>
    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        // 防止场景初始化时播放音效
        if (canPlaySFX == false)
            return;

        // 距离检测：音效源与玩家距离超过阈值时不播放，如果不需要检测距离，传入null即可
        if (_source != null &&
            Vector2.Distance(PlayerManager.Instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;

        // 防止数组越界
        if (_sfxIndex < sfx.Length)
        {
            // 动态音调：每次播放微调音高（避免重复音效机械感）
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    // 立即停止指定音效
    public void StopSFX(int _index) => sfx[_index].Stop();

    // 淡出停止：使音效平滑消失（避免突然切断的突兀感）
    public void StopSFXWithTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume; // 保存原始音量用于恢复

        // 指数级降低音量：每0.6秒降低当前音量的20%
        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.6f);

            // 达到静音阈值时完全停止
            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume; // 重置音量避免影响后续播放
                break;
            }
        }
    }

    // 随机播放BGM：用于场景切换或多样化的音乐选择
    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length); // 随机选择曲目
        PlayBGM(bgmIndex);
    }
    // 可以在某些时候播放指定的BGM
    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex; // 更新当前索引
        StopAllBGM();         // 停止所有正在播放的BGM（防止叠音）
        bgm[bgmIndex].Play(); // 播放指定BGM
    }

    // 停止所有背景音乐：切换曲目时使用
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    // 启用音效的安全解锁方法（由Invoke调用）
    private void AllowSFX() => canPlaySFX = true;
}
