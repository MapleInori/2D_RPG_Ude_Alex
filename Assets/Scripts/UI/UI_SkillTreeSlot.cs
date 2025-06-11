using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;

    /// <summary>
    /// 当前技能是否解锁
    /// </summary>
    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        // TODO:待添加：右键取消学习技能，重新锁定该技能，可以更方便测试.
        // TODO：先解锁插槽，然后解锁技能，技能那里判断依据是插槽上的bool值，所以这里是Awake，每个技能监听按钮是Start，但是其实不同脚本的Awake和Start顺序是不同的，应该换一种方式
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }
    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if (unlocked)
            skillImage.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UnlockSkillSlot()
    {
        if (PlayerManager.Instance.HaveEnoughMoney(skillCost) == false)
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }


        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        unlocked = true;
        skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription, skillName, skillCost);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

}
