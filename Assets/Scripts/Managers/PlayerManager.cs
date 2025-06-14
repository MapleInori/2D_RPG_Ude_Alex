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
        // ��ʹ���PlayerManager��_instance�ǹ���ģ���ͬһ����������������ʵ�_instance��ͬһ��
        // �����ж��PlayerManagerʱ��_instance���õ��ǵ�һ��ʵ����ÿ��ʵ�������жϺ󣬻Ὣ�󴴽���ɾ�������󴴽���Manager�Լ�ɾ�����Լ���
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        //DontDestroyOnLoad(gameObject); // ��������Ƿ�糡������������player�������գ���ʱ��������PlayerManager
    }


    private void Start()
    {
        currency += 1234;
    }
    private void Update()
    {
        // �����¼��س���ʱ����һᶪʧ��������Ӹ��ж�
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
    /// ʵ�ʿ�����Ҫô��ר�ŵ����������Ҫô����ǽ������������¼򵥴���һ��
    /// </summary>
    public void CheckPlayerDie()
    {
        if (player.transform.position.y <-1000)
        {
            player.stats.KillSelf();
        }
    }
}
