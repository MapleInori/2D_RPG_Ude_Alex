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
    /// 单个物品只判断一次是否掉落并且掉落一个，TODO：后续改为一种物品可掉落多个，列表单位不删除，总量为一个区间，每种物品掉落量为一个区间
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
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];//TODO:索引可能溢出？有时候会报错，为什么。似乎是死亡的时候掉落之后又触发了一下？

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
