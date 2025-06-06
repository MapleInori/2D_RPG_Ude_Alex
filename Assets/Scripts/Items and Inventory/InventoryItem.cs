using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
/// <summary>
/// ������Ʒ
/// </summary>
public class InventoryItem 
{
    public ItemData data;   // �����Ʒ��ʲô
    public int stackSize;   // �����Ʒ������


    public InventoryItem(ItemData itemdata)
    {
        data = itemdata;
        AddStack(); // ���һ����Ʒ������
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
