using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Inventory : MonoBehaviour, ISaveManager
{
    #region Instance
    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            return instance;
        }
    }
    public static int num = 0;
    #endregion

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;   // 装备栏
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;
    public List<InventoryItem> inventory;  // 背包中的物品 列表
    public Dictionary<ItemData, InventoryItem> inventoryDictionary; // 背包中的物品 字典
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Item Cooldown")]
    public float flaskCooldown;
    private float armorCooldown;
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;


    [Header("Data base")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    #region Awake
    private void Awake()
    {
        // 测试调试
        //Debug.Log(num++ + gameObject.name);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        // 应该等待读档结束再执行，教程没遇到这个问题，很奇怪
        //AddStartingItems();
        Invoke("AddStartingItems",1); 
    }

    private void AddStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        //Debug.Log("加载的物品的数量"+loadedItems.Count);
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    // AddItem会区分是材料还是装备，将它们添加到对应的列表中
                    AddItem(item.data);
                }
            }
            //Debug.Log("从存档加载物品");
            return;
        }

        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
            {
                AddItem(startingItems[i]);

            }
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
            }
        }
        if (oldEquipment != null)
        {
            //卸下装备，放入背包
            UnEquipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);
        UpdateSlotUI();
    }

    public void UnEquipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
            UpdateSlotUI();
        }
    }

    /// <summary>
    /// UI相关任何信息发生变化时
    /// </summary>
    private void UpdateSlotUI()
    {
        //TODO:为什么死亡掉落装备之后，装备UI没有消失？

        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                {
                    equipmentSlot[i].UpdateSlot(item.Value);
                    //Debug.Log(equipmentSlot[i].name);
                }
            }
        }
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }
        UpdateStatsUI();
    }
    /// <summary>
    /// 更新角色信息面板
    /// </summary>
    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    /// <summary>
    /// TODO:捡东西时会出问题，如果背包满了，东西消失但是没捡到，背包判满应该在捡的时候判断
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(item);

        }
        else if (item.itemType == ItemType.Material)
        {
            AddToStash(item);
        }


        UpdateSlotUI();
    }

    private void AddToStash(ItemData item)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(item);
            stash.Add(newItem);
            stashDictionary.Add(item, newItem);
        }
    }

    private void AddToInventory(ItemData item)
    {
        // 如果背包内有同名物品，数量加1
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else// 如果没有同名物品，新建该物品添加进去
        {
            InventoryItem newItem = new InventoryItem(item);
            inventory.Add(newItem);
            inventoryDictionary.Add(item, newItem);
        }
    }

    /// <summary>
    /// 从背包中移除物品
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ItemData item)
    {
        // 背包中是否存在该物品
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            // 如果只有一个，则从背包中移除该物品
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(item);
            }
            else//如果有多个，则该物品数量减一
            {
                value.RemoveStack();
                // InventoryItem为类，所以在传递过程中是引用，只需要value的数量减一，字典里该value的数量也会减一
            }
        }
        if (stashDictionary.TryGetValue(item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }
        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("Inventory is full");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 合成装备，TODO：但是没有解决同一种材料消耗多个的问题，后续再修复
    /// </summary>
    /// <param name="_itemToCraft"></param>
    /// <param name="_requiredMaterials"></param>
    /// <returns></returns>
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log(_requiredMaterials[i].data.itemName + " not enough.");
                    return false;
                }
            }
            else
            {
                Debug.Log(_requiredMaterials[i].data.itemName + " not enough.");
                return false;
            }
        }

        // If all materials are avalible, remove them from stash.

        foreach (var requiredMaterial in _requiredMaterials)
        {
            for (int i = 0; i < requiredMaterial.stackSize; i++)
            {
                RemoveItem(requiredMaterial.data);
            }
        }

        AddItem(_itemToCraft);
        Debug.Log("Craft is succsesful: " + _itemToCraft.name);
        return true;
    }

    public List<InventoryItem> GetEquipmentList()
    {
        return equipment;
    }

    public List<InventoryItem> GetStashLish()
    {
        return stash;
    }

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipmentItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
            {
                equipmentItem = item.Key;
            }
        }

        return equipmentItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;
        // Tips:在这里比较falskCooldown会避免刚开始无法使用道具的问题，因为一开始的时候Time.time是0，如果后边比较的cd是物品真实CD，就会出现开局无法使用
        // 比如道具CD15秒，那么游戏前15秒将无法使用这个道具。
        // 修改为下方写法之后，开局使用道具之前，flaskCooldown是空的，所以是0，在使用之后在设置为使用的道具的CD即可
        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown;");
    }
    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown");
        return false;
    }

    public void LoadData(GameData _data)
    {
        //Debug.Log("读档物品");
        //FillUpItemDataBase(); 用于测试查看填充的数据
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            // 需要手动填充itemDataBase，右键该脚本，选择"Fill up item data base"，会将Assets/Data/Items下的所有ItemData填充到itemDataBase中
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;
                    loadedItems.Add(itemToLoad);
                }
            }
        }
        //Debug.Log("读档物品数量"+loadedItems.Count);
        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            //Debug.Log(pair.Key.itemId);
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            //Debug.Log(pair.Key.itemId);
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            //Debug.Log(pair.Key.itemId);
            _data.equipmentId.Add(pair.Key.itemId);
        }
        //Debug.Log("保存物品");
    }

#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}
