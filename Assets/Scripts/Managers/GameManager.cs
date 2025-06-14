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

        //DontDestroyOnLoad(gameObject); ���ٱ��������¼��س���ʱ���»�ȡ�������
    }
    private void Start()
    {
        // ��ȡ���м���
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
    // TODO�����¼��س����ᵼ�������ʧ���Ӷ�����PlayerManager.Instance.playerΪnull,���¸��ֵط���player���ö�ʧ
    // DONE��ѡ����PlayerManager�з������player�Ƿ�����ˣ������ھ��ҵ�����ֵ�ͺ���
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
        // ��Ȼ���������ҵ�λ�ã�������Ȼֻ�����������ʬ�壬��Ϊ���������ʱ���Ż��е����Ǯ�����е����Ǯ�ǲŻ�����ʬ�壬��������ʬ��󣬵����Ǯ������Ϊ0
        // ����ٴ��������Ḳ���ϴε�ʬ�壬������񣬾���ʧ�ˣ��������˳��浵������
        // ��Ӷ����жϣ����ȷʵ�е��䣬�ٸ���λ�ã����û�е��䣬�����ϴ��������������ھͻᵼ��0��Ǯ�������Ḳ���ϴ�ʬ�壬�������ˣ�û��ϵ
        // ����������˴��ţ��ټӸ������жϾ�����
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
    /// ���ؼ�������
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
    /// �����������ļ���λ�ã������λ������Ϊ�ü����λ��
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
                // Alexԭ��������ʹ��PlayerManager.instance.player����ֵ���������������⣬���¼��س���ʱ����ᱻ����ֵ��
                // û�취�����¼��س���������Player��ֻ����PlayerManager�з������player�Ƿ�����ˣ������ھ��ҵ�����ֵ�ͺ���
                //player.transform.position = checkpoint.transform.position;
                if (PlayerManager.Instance.player == null)
                {
                    PlayerManager.Instance.player = GameObject.Find("Player").GetComponent<Player>();
                }
                // TODO:���¼��س������ᵼ������ļ��㶪ʧ��CNM
                // DONE:DontDestroyOnLoad(gameObject)���µĺ���
                PlayerManager.Instance.player.transform.position = checkpoint.transform.position + new Vector3(0, 2.5f, 0);// ���������ڵײ�����Ҫ����ƫ��
            }
        }
    }
    /// <summary>
    /// �ҵ������������ļ���ļ���
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
