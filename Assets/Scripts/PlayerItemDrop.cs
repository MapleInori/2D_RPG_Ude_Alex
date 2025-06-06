using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    /// <summary>
    /// ��������ʱ��Ʒ���䡣TODO��װ��UIû�и��£��������Ե������ж����������װ���ֵ�û�����⣬ȷʵ���Ƴ��ˣ�Ӧ���ǵ����ˣ�����ΪʲôUIû����
    /// </summary>
    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.Instance;

        List<InventoryItem> currentStash = inventory.GetStashLish();
        List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        List<InventoryItem> itemsToUnequip = new List<InventoryItem> ();
        List<InventoryItem> materialsToLoose = new List<InventoryItem> ();


        foreach(InventoryItem item in currentEquipment)
        {
            if(Random.Range(0,100) < chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }
        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnEquipItem(itemsToUnequip[i].data as ItemData_Equipment);
            //Debug.Log("???");
        }

        foreach (InventoryItem item in currentStash)
        {
            if(Random.Range(0,100)< chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }
        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }

    }
}
