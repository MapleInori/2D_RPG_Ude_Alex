using System.Text;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

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
    public string itemId;


    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    // TODO���ʼǣ�DS GUIDΨһ������
    private void OnValidate()
    {
#if UNITY_EDITOR
        // �ҵ���ǰ��Դ���ڵ�·��
        string path = AssetDatabase.GetAssetPath(this);
        // �ҵ�·�����ڵ���Դ��������·������һ��ΨһGUID
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }


    public virtual string GetDescription()
    {
        return "";
    }
}
