using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get { return _instance; }
    }

    public Player player;

    private void Awake()
    {
        // 处理重复实例。差点晕了，多个即使多个PlayerManager，_instance是共享的，是同一个，所以在这里访问的_instance是同一个
        // 而当有多个PlayerManager时，_instance引用的是第一个实例，每个实例进行判断后，会将后创建的删除掉（后创建的Manager自己删除掉自己）
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // 按需决定是否跨场景保留
    }




}
