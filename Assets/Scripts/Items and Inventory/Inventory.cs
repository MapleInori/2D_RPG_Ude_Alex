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

    public List<InventoryItem> equipment;   // װ����
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;
    public List<InventoryItem> inventory;  // �����е���Ʒ �б�
    public Dictionary<ItemData, InventoryItem> inventoryDictionary; // �����е���Ʒ �ֵ�
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
        // ���Ե���
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

        // Ӧ�õȴ�����������ִ�У��̳�û����������⣬�����
        //AddStartingItems();
        Invoke("AddStartingItems",1); 
    }

    private void AddStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        //Debug.Log("���ص���Ʒ������"+loadedItems.Count);
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    // AddItem�������ǲ��ϻ���װ������������ӵ���Ӧ���б���
                    AddItem(item.data);
                }
            }
            //Debug.Log("�Ӵ浵������Ʒ");
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
            //ж��װ�������뱳��
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
    /// UI����κ���Ϣ�����仯ʱ
    /// </summary>
    private void UpdateSlotUI()
    {
        //TODO:Ϊʲô��������װ��֮��װ��UIû����ʧ��

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
    /// ���½�ɫ��Ϣ���
    /// </summary>
    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    /// <summary>
    /// TODO:����ʱ������⣬����������ˣ�������ʧ����û�񵽣���������Ӧ���ڼ��ʱ���ж�
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
        // �����������ͬ����Ʒ��������1
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else// ���û��ͬ����Ʒ���½�����Ʒ��ӽ�ȥ
        {
            InventoryItem newItem = new InventoryItem(item);
            inventory.Add(newItem);
            inventoryDictionary.Add(item, newItem);
        }
    }

    /// <summary>
    /// �ӱ������Ƴ���Ʒ
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ItemData item)
    {
        // �������Ƿ���ڸ���Ʒ
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            // ���ֻ��һ������ӱ������Ƴ�����Ʒ
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(item);
            }
            else//����ж���������Ʒ������һ
            {
                value.RemoveStack();
                // InventoryItemΪ�࣬�����ڴ��ݹ����������ã�ֻ��Ҫvalue��������һ���ֵ����value������Ҳ���һ
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
    /// �ϳ�װ����TODO������û�н��ͬһ�ֲ������Ķ�������⣬�������޸�
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
        // Tips:������Ƚ�falskCooldown�����տ�ʼ�޷�ʹ�õ��ߵ����⣬��Ϊһ��ʼ��ʱ��Time.time��0�������߱Ƚϵ�cd����Ʒ��ʵCD���ͻ���ֿ����޷�ʹ��
        // �������CD15�룬��ô��Ϸǰ15�뽫�޷�ʹ��������ߡ�
        // �޸�Ϊ�·�д��֮�󣬿���ʹ�õ���֮ǰ��flaskCooldown�ǿյģ�������0����ʹ��֮��������Ϊʹ�õĵ��ߵ�CD����
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
        //Debug.Log("������Ʒ");
        //FillUpItemDataBase(); ���ڲ��Բ鿴��������
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            // ��Ҫ�ֶ����itemDataBase���Ҽ��ýű���ѡ��"Fill up item data base"���ὫAssets/Data/Items�µ�����ItemData��䵽itemDataBase��
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
        //Debug.Log("������Ʒ����"+loadedItems.Count);
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
        //Debug.Log("������Ʒ");
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
