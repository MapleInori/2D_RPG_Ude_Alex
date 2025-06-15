using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // TODO:�ʼǣ�����ģʽ�ĸ���д�������д�Ľ�׳һЩ
    // ʹ�õ���ģʽȷ��ȫ��ֻ��һ��AudioManagerʵ��
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                // ���Դӳ����в���
                instance = FindObjectOfType<AudioManager>();

                // ����������򴴽�
                if (instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    instance = obj.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    // ��Ч��С���ž��룺�����˾������Ч�������ԣ��Ż����ܣ�
    [SerializeField] private float sfxMinimumDistance;
    // ��ЧԴ���飺���������Ч��AudioSource���
    [SerializeField] private AudioSource[] sfx;
    // ��������Դ���飺�������BGM��AudioSource���
    [SerializeField] private AudioSource[] bgm;

    // TODO���ʼǣ���Ч���Ź�������Щ���������ָ����أ�
    // Ҳ�����ø���ϸ�ķ��飬�����Ƹ�����Ч�Ĳ��ţ�������Inspector���õ�ʱ�������������Ҳ��Ū����������Ĵ���д����Ӧ�ûḴ��һЩ
    // ����ÿ������������һ����������������������ʲô��Ч��������Inspector�и�ֱ�ۣ��ڵ��õ�ʱ������������ɣ�
    // ��������Ҳ�����ӣ�Ҳ���õ���ʲô����������Ч˳����ң�Ȼ���¸����������ŵ���Чȫ������
    // ֻ���������Ķ�����ȫȡ������Ч�����������Ч���࣬���÷ǳ��߳�.
    // �������Ч���ڿ����õ��ĵط�������PlayerOnAwake������ʱ�Զ����ţ����Ƽ���Ҳ��һ�ַ������������ַ����ᵼ����Ч�����ɢ�ڸ����ű��У�����ά����

    // BGM�ܿ��أ����Ʊ��������Ƿ񲥷�
    public bool playBgm;
    // ��ǰ���ŵ�BGM����
    private int bgmIndex;

    // SFX��ȫ������ֹ��Ϸ��ʼ��ʱ����������Ч�����ⳡ������������
    private bool canPlaySFX;

    private void Awake()
    {
        // ������ʼ����ȷ��������ڶ��AudioManagerʵ��
        if (instance != null && instance != this)
            Destroy(gameObject); // ���ٵ�ǰʵ�������ͻ
        else
            instance = this; // �״δ���ʱ����ʵ������
        // ���¼��ػ������Ч�б��������ǣ�����Ϊ���õ���Ч��Դ���ڳ����У��������е���Ч���������ؽ��ˣ����Ե��������б�����ˣ���ȡ����һ�м���
        //DontDestroyOnLoad(gameObject);
        // �ӳ�1��������Ч�����ⳡ������ʱ����������Ч�����ڿ�ʼһ˲���򿪼�������֤�������������ݼ��أ����������ӳ�һ�룬��Ȼ�տ�ʼ����һ���л�����
        Invoke("AllowSFX", 1f);
    }

    private void Update()
    {
        // BGM״̬���������ܿ��ؿ��Ʊ������ֲ���
        if (!playBgm)
            StopAllBGM(); // �ر�����BGM
        else
        {
            // �Զ���������ǰBGM����ֹͣʱ���²��ţ���ֹ�����жϣ�
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }
    /// <summary>
    /// ������Ч��������������ָ������Ч���������Ҫ�����⣬���Դ���null��ΪԴ����
    /// </summary>
    /// <param name="_sfxIndex"></param>
    /// <param name="_source"></param>
    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        // ��ֹ������ʼ��ʱ������Ч
        if (canPlaySFX == false)
            return;

        // �����⣺��ЧԴ����Ҿ��볬����ֵʱ�����ţ��������Ҫ�����룬����null����
        if (_source != null &&
            Vector2.Distance(PlayerManager.Instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;

        // ��ֹ����Խ��
        if (_sfxIndex < sfx.Length)
        {
            // ��̬������ÿ�β���΢�����ߣ������ظ���Ч��е�У�
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    // ����ָֹͣ����Ч
    public void StopSFX(int _index) => sfx[_index].Stop();

    // ����ֹͣ��ʹ��Чƽ����ʧ������ͻȻ�жϵ�ͻأ�У�
    public void StopSFXWithTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defaultVolume = _audio.volume; // ����ԭʼ�������ڻָ�

        // ָ��������������ÿ0.6�뽵�͵�ǰ������20%
        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.6f);

            // �ﵽ������ֵʱ��ȫֹͣ
            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume; // ������������Ӱ���������
                break;
            }
        }
    }

    // �������BGM�����ڳ����л��������������ѡ��
    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length); // ���ѡ����Ŀ
        PlayBGM(bgmIndex);
    }
    // ������ĳЩʱ�򲥷�ָ����BGM
    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex; // ���µ�ǰ����
        StopAllBGM();         // ֹͣ�������ڲ��ŵ�BGM����ֹ������
        bgm[bgmIndex].Play(); // ����ָ��BGM
    }

    // ֹͣ���б������֣��л���Ŀʱʹ��
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    // ������Ч�İ�ȫ������������Invoke���ã�
    private void AllowSFX() => canPlaySFX = true;
}
