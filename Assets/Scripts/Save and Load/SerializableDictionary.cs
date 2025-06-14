using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����һ�������л����ֵ��࣬�̳��� Dictionary<TKey, TValue> ��ʵ�� ISerializationCallbackReceiver �ӿ�
// ��ʹ���������� Unity �� Inspector ��������ʾ��֧�����л�
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // �������л��洢�ļ��б�
    [SerializeField] private List<TKey> keys = new List<TKey>();
    // �������л��洢��ֵ�б�
    [SerializeField] private List<TValue> values = new List<TValue>();

    // �����л�֮ǰ���õķ���
    public void OnBeforeSerialize()
    {
        // ������еļ�ֵ�б�
        keys.Clear();
        values.Clear();

        // �����ֵ��е����м�ֵ��
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            // ������ӵ� keys �б�
            keys.Add(pair.Key);
            // ��ֵ��ӵ� values �б�
            values.Add(pair.Value);
        }
    }

    // �ڷ����л�֮����õķ���
    public void OnAfterDeserialize()
    {
        // ��յ�ǰ�ֵ�����
        this.Clear();

        // ����ֵ�����Ƿ�ƥ�䣬�����ƥ�����������
        if (keys.Count != values.Count)
        {
            Debug.Log("Keys count is not equal to values count");
        }

        // �������б�����ֵ��������ӵ��ֵ���
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}