using System.Text;
using UnityEngine;

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

    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    public virtual string GetDescription()
    {
        return "";
    }
}
