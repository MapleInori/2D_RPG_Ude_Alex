using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 物品类型，可以扩展道具
/// </summary>
public enum ItemType
{
    Material,
    Equipment,
}

/// <summary>
/// 物品基类，仅有物品类型，名称和图标
/// </summary>
[CreateAssetMenu(fileName = "New Item Data",menuName ="Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;


    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    // TODO：笔记，DS GUID唯一性问题
    private void OnValidate()
    {
#if UNITY_EDITOR
        // 找到当前资源所在的路径
        string path = AssetDatabase.GetAssetPath(this);
        // 找到路径所在的资源，并根据路径返回一个唯一GUID
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }


    public virtual string GetDescription()
    {
        return "";
    }
}
