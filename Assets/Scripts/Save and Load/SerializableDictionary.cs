using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这是一个可序列化的字典类，继承自 Dictionary<TKey, TValue> 并实现 ISerializationCallbackReceiver 接口
// 这使得它可以在 Unity 的 Inspector 窗口中显示并支持序列化
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // 用于序列化存储的键列表
    [SerializeField] private List<TKey> keys = new List<TKey>();
    // 用于序列化存储的值列表
    [SerializeField] private List<TValue> values = new List<TValue>();

    // 在序列化之前调用的方法
    public void OnBeforeSerialize()
    {
        // 清空现有的键值列表
        keys.Clear();
        values.Clear();

        // 遍历字典中的所有键值对
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            // 将键添加到 keys 列表
            keys.Add(pair.Key);
            // 将值添加到 values 列表
            values.Add(pair.Value);
        }
    }

    // 在反序列化之后调用的方法
    public void OnAfterDeserialize()
    {
        // 清空当前字典内容
        this.Clear();

        // 检查键值数量是否匹配，如果不匹配则输出警告
        if (keys.Count != values.Count)
        {
            Debug.Log("Keys count is not equal to values count");
        }

        // 遍历键列表，将键值对重新添加到字典中
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}