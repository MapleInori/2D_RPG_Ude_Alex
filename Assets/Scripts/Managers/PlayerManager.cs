using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get { return _instance; }
    }

    public Player player;

    public int currency;
    private void Awake()
    {
        // 即使多个PlayerManager，_instance是共享的，是同一个，所以在这里访问的_instance是同一个
        // 而当有多个PlayerManager时，_instance引用的是第一个实例，每个实例进行判断后，会将后创建的删除掉（后创建的Manager自己删除掉自己）
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        //DontDestroyOnLoad(gameObject); // 按需决定是否跨场景保留，由于player经常报空，暂时决定保留PlayerManager
    }


    private void Start()
    {
        currency += 1234;
    }
    private void Update()
    {
        // 当重新加载场景时，玩家会丢失，所以添加该判断
        if(player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
        CheckPlayerDie();
    }
    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency = currency - _price;
        return true;
    }

    public float GetCurrency()
    {
        return currency;
    }

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }

    /// <summary>
    /// 实际开发后，要么有专门的死亡情况，要么空气墙，这里针对跳崖简单处理一下
    /// </summary>
    public void CheckPlayerDie()
    {
        if (player.transform.position.y <-1000)
        {
            player.stats.KillSelf();
        }
    }
}
