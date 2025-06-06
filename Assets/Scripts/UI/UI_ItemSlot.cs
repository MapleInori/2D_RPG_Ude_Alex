using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 物品槽
/// </summary>
public class UI_ItemSlot : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image itemImage;   // 该物品槽的物品图标
    [SerializeField] private TextMeshProUGUI itemText;  // 该物品槽的物品数量

    protected UI ui;
    public InventoryItem item;  // 该物品槽的物品


    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    /// <summary>
    /// 更新当前物品槽，所以需要告诉当前物品槽是哪个物品
    /// </summary>
    /// <param name="newItem"></param>
    public void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;
        // 没有物品时为透明色，所以有物品后设置为白色
        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    /// <summary>
    /// 左右键点击时执行
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null) return;
        Debug.Log("Click Item :" + item.data.itemName);
        if(Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.Instance.RemoveItem(item.data );
            return;
        }

        if(item.data.itemType == ItemType.Equipment)
        {
            Inventory.Instance.EquipItem(item.data);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null)
            return;

        ui.itemToolTip.HideToolTip();
    }
}
