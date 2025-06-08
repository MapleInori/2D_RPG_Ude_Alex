using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash")]    // ���ܱ���ͷ
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton; // ���ܶ�Ӧ�������ϵĲ��
    [SerializeField] public bool dashUnlocked { get; private set; }  // �ü����Ƿ����ʹ��

    [Header("Clone on dash")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    [SerializeField] public bool cloneOnDashUnlocked { get; private set; }

    [Header("Clone on arrival")]
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    [SerializeField] public bool cloneOnArrivalUnlocked { get; private set; }


    public override void UseSkill()
    {
        base.UseSkill();
    }

    protected override void Start()
    {
        base.Start();
        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    private void UnlockDash()
    {
        // �������Ͻ��������ܱ�������ʹ�ã�������
        if (dashUnlockButton.unlocked)
            dashUnlocked = true;
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
            cloneOnDashUnlocked = true;
    }

    private void UnlockCloneOnArrival()
    {
        if (cloneOnArrivalUnlockButton.unlocked)
            cloneOnArrivalUnlocked = true;
    }

    public void CreateCloneOnDashStart()
    {
        if (cloneOnDashUnlocked)
        {
            SkillManager.Instance.clone.CreateClone(player.transform, Vector2.zero);
        }
    }

    public void CreateCloneOnDashOver()
    {
        if (cloneOnArrivalUnlocked)
        {
            SkillManager.Instance.clone.CreateClone(player.transform, Vector2.zero);
        }
    }
}
