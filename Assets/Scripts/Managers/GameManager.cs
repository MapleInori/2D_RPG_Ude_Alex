using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{

    private static GameManager instance;
    public static GameManager Instance => instance;

    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Dead Body With Currency Info")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;
    private bool pasuedGame;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        //DontDestroyOnLoad(gameObject); 不再保留，重新加载场景时重新获取检查点情况
    }
    private void Start()
    {
        // 获取所有检查点
        checkpoints = FindObjectsOfType<Checkpoint>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            RestartScene();

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!pasuedGame)
            {
                pasuedGame = true;
                GameManager.instance.PauseGame(pasuedGame);
            }
            else
            {
                pasuedGame = false;
                GameManager.instance.PauseGame(pasuedGame);
            }

        }
    }
    // TODO：重新加载场景会导致玩家消失，从而导致PlayerManager.Instance.player为null,导致各种地方的player引用丢失
    // DONE：选择在PlayerManager中反复检查player是否存在了，不存在就找到并赋值就好了
    public void RestartScene()
    {
        SaveManager.Instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        SaveManager.Instance.LoadGame();
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadWithDelay(_data));
    }

    public void SaveData(ref GameData _data)
    {
        // 虽然保存的是玩家的位置，但是依然只有死亡会出现尸体，因为当玩家死亡时，才会有掉落金钱，当有掉落金钱是才会生成尸体，并且生成尸体后，掉落金钱被重置为0
        // 如果再次死亡，会覆盖上次的尸体，如果不捡，就消失了，会由于退出存档而覆盖
        // 添加额外判断，如果确实有掉落，再覆盖位置，如果没有掉落，则保留上次死亡，不过现在就会导致0金钱死亡不会覆盖上次尸体，不过算了，没关系
        // 这个问题无伤大雅，再加个额外判断就行了
        if (lostCurrencyAmount > 0)
        {
            _data.lostCurrencyAmount = lostCurrencyAmount;
            _data.lostCurrencyX = PlayerManager.Instance.player.transform.position.x;
            _data.lostCurrencyY = PlayerManager.Instance.player.transform.position.y;

        }


        if (FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().id;

        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }
    /// <summary>
    /// 加载检查点数据
    /// </summary>
    /// <param name="_data"></param>
    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    /// <summary>
    /// 加载最近激活的检查点位置，将玩家位置设置为该检查点的位置
    /// </summary>
    /// <param name="_data"></param>
    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;


        closestCheckpointId = _data.closestCheckpointId;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
            {
                // Alex原本在这里使用PlayerManager.instance.player来赋值，后来发现有问题，重新加载场景时这里会被赋空值，
                // 没办法，重新加载场景会销毁Player，只好在PlayerManager中反复检查player是否存在了，不存在就找到并赋值就好了
                //player.transform.position = checkpoint.transform.position;
                if (PlayerManager.Instance.player == null)
                {
                    PlayerManager.Instance.player = GameObject.Find("Player").GetComponent<Player>();
                }
                // TODO:重新加载场景还会导致这里的检查点丢失，CNM
                // DONE:DontDestroyOnLoad(gameObject)导致的好像
                PlayerManager.Instance.player.transform.position = checkpoint.transform.position + new Vector3(0, 2.5f, 0);// 检查点轴心在底部，需要向上偏移
            }
        }
    }
    /// <summary>
    /// 找到距离玩家最近的激活的检查点
    /// </summary>
    /// <returns></returns>
    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.Instance.player.transform.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }


}
