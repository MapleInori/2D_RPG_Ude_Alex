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
        // �����ظ�ʵ����������ˣ������ʹ���PlayerManager��_instance�ǹ���ģ���ͬһ����������������ʵ�_instance��ͬһ��
        // �����ж��PlayerManagerʱ��_instance���õ��ǵ�һ��ʵ����ÿ��ʵ�������жϺ󣬻Ὣ�󴴽���ɾ�������󴴽���Manager�Լ�ɾ�����Լ���
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // ��������Ƿ�糡������
    }




}
