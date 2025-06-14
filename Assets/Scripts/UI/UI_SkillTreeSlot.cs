using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
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

    /// <summary>
    /// 每个技能从存档中读取学习数据，将技能的解锁状态设置为存档中的值
    /// </summary>
    /// <param name="_data"></param>
    public void LoadData(GameData _data)
    {
        //Debug.Log("加载技能 " + skillName + " 的状态");
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }
    /// <summary>
    /// 每个技能将自己的解锁状态保存到存档中
    /// </summary>
    /// <param name="_data"></param>
    public void SaveData(ref GameData _data)
    {
        //Debug.Log("存档技能");
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            // 如果存档中已经有该技能的解锁状态，则更新它
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
            _data.skillTree.Add(skillName, unlocked);

        //Debug.Log("Skill " + skillName + " saved with status: " + unlocked);
    }
}
