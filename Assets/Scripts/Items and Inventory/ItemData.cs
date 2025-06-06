using System.Text;
using UnityEngine;

/// <summary>
/// ��Ʒ���ͣ�������չ����
/// </summary>
public enum ItemType
{
    Material,
    Equipment,
}

/// <summary>
/// ��Ʒ���࣬������Ʒ���ͣ����ƺ�ͼ��
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
