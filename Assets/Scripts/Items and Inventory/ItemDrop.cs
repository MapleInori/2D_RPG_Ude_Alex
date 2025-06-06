using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    /// <summary>
    /// ������Ʒֻ�ж�һ���Ƿ���䲢�ҵ���һ����TODO��������Ϊһ����Ʒ�ɵ��������б�λ��ɾ��������Ϊһ�����䣬ÿ����Ʒ������Ϊһ������
    /// </summary>
    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if(Random.Range(0,100) < possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];//TODO:���������������ʱ��ᱨ��Ϊʲô���ƺ���������ʱ�����֮���ִ�����һ�£�

            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _item)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-3, 3), Random.Range(12,15));

        newDrop.GetComponent<ItemObject>().SetupItem(_item, randomVelocity);
    
    }
}
