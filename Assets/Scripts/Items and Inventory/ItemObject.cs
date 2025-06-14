
using UnityEngine;

public class ItemObject : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Vector2 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void SetupVisual()
    {
        if (itemData == null)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Object - " + itemData.itemName;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
        SetupVisual();
    }

    /// <summary>
    /// TODO:��װ��û�����ˣ���ô����ǲ���������ڱ��������أ�
    /// </summary>
    public void PickUpItem()
    {
        if (!Inventory.Instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 5);
            return;
        }
        //Debug.Log("Picked Up " + itemData.itemName);
        AudioManager.Instance.PlaySFX(9, transform);
        Inventory.Instance.AddItem(itemData);

        Destroy(gameObject);
    }
}
