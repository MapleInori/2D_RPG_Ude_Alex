using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��Ϸ�����࣬���ڴ洢�͹�����Ϸ�ĸ���״̬�ͽ�������
// ���Ϊ�����л����Ա���Ա��浽�ļ�����Unity�в鿴
[System.Serializable]
public class GameData
{
    // ��ҵ�ǰӵ�еĻ�������
    public int currency;

    // ������״̬�ֵ䣺���Ǽ���ID��ֵ��ʾ�Ƿ��ѽ�����true/false��
    public SerializableDictionary<string, bool> skillTree;

    // ��Ʒ����ֵ䣺������ƷID��ֵ�Ǹ���Ʒ������
    public SerializableDictionary<string, int> inventory;

    // ��ǰװ������ƷID�б�
    public List<string> equipmentId;

    // ����״̬�ֵ䣺���Ǽ���ID��ֵ��ʾ�Ƿ��Ѽ��true/false��
    public SerializableDictionary<string, bool> checkpoints;

    // �������ļ���ID
    public string closestCheckpointId;

    // �������ʱ��ʧ���ҵ�λ��X����
    public float lostCurrencyX;
    // �������ʱ��ʧ���ҵ�λ��Y����
    public float lostCurrencyY;
    // �������ʱ��ʧ�Ļ�������
    public int lostCurrencyAmount;

    // ���������ֵ䣺������Ƶ���ͣ���"Master"��"Music"�ȣ���ֵ��������С��0-1��
    public SerializableDictionary<string, float> volumeSettings;

    // ���캯������ʼ�����������ֶ�
    public GameData()
    {
        // ��ʼ����ʧ�����������
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        // ��ʼ����������
        this.currency = 0;

        // ��ʼ�������ֵ���б�
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        // ��ʼ�������������
        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();

        // ��ʼ����������
        volumeSettings = new SerializableDictionary<string, float>();
    }
}