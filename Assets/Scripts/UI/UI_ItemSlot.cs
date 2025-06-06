using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��Ʒ��
/// </summary>
public class UI_ItemSlot : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image itemImage;   // ����Ʒ�۵���Ʒͼ��
    [SerializeField] private TextMeshProUGUI itemText;  // ����Ʒ�۵���Ʒ����

    protected UI ui;
    public InventoryItem item;  // ����Ʒ�۵���Ʒ


    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    /// <summary>
    /// ���µ�ǰ��Ʒ�ۣ�������Ҫ���ߵ�ǰ��Ʒ�����ĸ���Ʒ
    /// </summary>
    /// <param name="newItem"></param>
    public void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;
        // û����ƷʱΪ͸��ɫ����������Ʒ������Ϊ��ɫ
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
    /// ���Ҽ����ʱִ��
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
