using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 存档管理器类，负责游戏数据的保存和加载
public class SaveManager : MonoBehaviour
{
    // 单例实例，确保全局只有一个存档管理器
    private static SaveManager instance;
    public static SaveManager Instance => instance;

    [SerializeField] private string fileName;       // 存档文件名
    [SerializeField] private bool encryptData;     // 是否加密存档数据
    private GameData gameData;                     // 当前游戏数据
    [SerializeField] private List<ISaveManager> saveManagers; // 所有需要参与存档的组件列表
    private FileDataHandler dataHandler;           // 文件数据处理器

    // Unity编辑器上下文菜单，用于测试删除存档文件
    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        // 实现单例模式
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
        //DontDestroyOnLoad(gameObject); // 按需决定是否跨场景保留
    }

    // Start方法在场景加载后调用
    private void Start()
    {
        // 初始化文件数据处理器
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        // 查找场景中所有实现ISaveManager接口的组件，等待其他加载完成再获取，不然会漏掉某些，比如技能树，为什么教程没漏？
        saveManagers = FindAllSaveManagers();

        // 加载游戏数据
        LoadGame();
    }

    // 创建新游戏数据
    public void NewGame()
    {
        // 新游戏时，实例化一个空的游戏数据
        gameData = new GameData();
    }

    // 加载游戏数据
    public void LoadGame()
    {
        // 从文件加载数据
        gameData = dataHandler.Load();

        // 如果没有存档数据，则创建新游戏
        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }
        //Debug.Log(gameData.ToString());
        // 通知所有需要加载数据的组件
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    // 保存游戏数据
    public void SaveGame()
    {
        // 收集所有需要保存的数据
        foreach (ISaveManager saveManager in saveManagers)
        {
            //Debug.Log(saveManager.ToString());
            saveManager.SaveData(ref gameData);
        }

        // 将数据保存到文件
        dataHandler.Save(gameData);
    }

    public void SaveAndExitGame()
    {
        SaveGame();
        Application.Quit();
    }

    // 应用退出时自动保存
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // 查找场景中所有实现ISaveManager接口的组件
    private List<ISaveManager> FindAllSaveManagers()
    {
        // TODO：笔记，添加True，包含未激活的对象。技能树一开始是未激活的，不加true会找不到，导致后续存档读档时无法调用，技能树无法加载和存档数据
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }

    // 检查是否存在存档数据
    public bool HasSavedData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }
}