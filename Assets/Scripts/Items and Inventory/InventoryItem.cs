using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
/// <summary>
/// 背包物品
/// </summary>
public class InventoryItem 
{
    public ItemData data;   // 这个物品是什么
    public int stackSize;   // 这个物品的数量


    public InventoryItem(ItemData itemdata)
    {
        data = itemdata;
        AddStack(); // 添加一个物品到背包
    }

    public void AddStack()
    {
        stackSize++;
    }

    public void RemoveStack()
    {
        stackSize--;
    }
}
