using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �浵�������࣬������Ϸ���ݵı���ͼ���
public class SaveManager : MonoBehaviour
{
    // ����ʵ����ȷ��ȫ��ֻ��һ���浵������
    private static SaveManager instance;
    public static SaveManager Instance => instance;

    [SerializeField] private string fileName;       // �浵�ļ���
    [SerializeField] private bool encryptData;     // �Ƿ���ܴ浵����
    private GameData gameData;                     // ��ǰ��Ϸ����
    [SerializeField] private List<ISaveManager> saveManagers; // ������Ҫ����浵������б�
    private FileDataHandler dataHandler;           // �ļ����ݴ�����

    // Unity�༭�������Ĳ˵������ڲ���ɾ���浵�ļ�
    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        // ʵ�ֵ���ģʽ
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        //DontDestroyOnLoad(gameObject); // ��������Ƿ�糡������
    }

    // Start�����ڳ������غ����
    private void Start()
    {
        // ��ʼ���ļ����ݴ�����
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        // ���ҳ���������ʵ��ISaveManager�ӿڵ�������ȴ�������������ٻ�ȡ����Ȼ��©��ĳЩ�����缼������Ϊʲô�̳�û©��
        saveManagers = FindAllSaveManagers();

        // ������Ϸ����
        LoadGame();
    }

    // ��������Ϸ����
    public void NewGame()
    {
        // ����Ϸʱ��ʵ����һ���յ���Ϸ����
        gameData = new GameData();
    }

    // ������Ϸ����
    public void LoadGame()
    {
        // ���ļ���������
        gameData = dataHandler.Load();

        // ���û�д浵���ݣ��򴴽�����Ϸ
        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }
        //Debug.Log(gameData.ToString());
        // ֪ͨ������Ҫ�������ݵ����
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    // ������Ϸ����
    public void SaveGame()
    {
        // �ռ�������Ҫ���������
        foreach (ISaveManager saveManager in saveManagers)
        {
            //Debug.Log(saveManager.ToString());
            saveManager.SaveData(ref gameData);
        }

        // �����ݱ��浽�ļ�
        dataHandler.Save(gameData);
    }

    public void SaveAndExitGame()
    {
        SaveGame();
        Application.Quit();
    }

    // Ӧ���˳�ʱ�Զ�����
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // ���ҳ���������ʵ��ISaveManager�ӿڵ����
    private List<ISaveManager> FindAllSaveManagers()
    {
        // TODO���ʼǣ����True������δ����Ķ��󡣼�����һ��ʼ��δ����ģ�����true���Ҳ��������º����浵����ʱ�޷����ã��������޷����غʹ浵����
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }

    // ����Ƿ���ڴ浵����
    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }
}