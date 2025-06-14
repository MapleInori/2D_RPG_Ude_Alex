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
        if(possibleDrop.Length == 0)
        {
            Debug.Log("Item Pool is Empty");
            return;
        }

        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if(Random.Range(0,100) < possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }
        // 似乎是因为没东西的原因？
        if(dropList.Count <=0 )
        {
            return;
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            // 终于给我Debug到了，物品掉完之后还会继续进行判断导致的索引溢出，其实这时候dropList.Count已经是0了，依然来了一次循环导致的报错
            if (dropList.Count <= 0)
            {
                return;
            }
            int randomNum = Random.Range(0, dropList.Count);//根据Range左闭右开特性，右边不需要减一
            ItemData randomItem = dropList[randomNum];//TODO:索引可能溢出？有时候会报错，为什么。似乎是死亡的时候掉落之后又触发了一下？DONE:上边解决了
            Debug.Log(i+":"+dropList.Count +"\n "+randomItem.itemName+" " + randomNum);
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
