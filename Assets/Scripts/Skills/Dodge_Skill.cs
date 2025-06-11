using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount; // 该技能增加的闪避率
    public bool dodgeUnlocked;

    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodge;
    public bool dodgeMirageUnlocked;


    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodge.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);
    }

    //protected override void CheckUnlock()
    //{
    //    UnlockDodge();
    //    UnlockMirageDodge();
    //}

    private void UnlockDodge()
    {
        // 避免反复解锁重复增加属性
        if (unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.Instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if (unlockMirageDodge.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
            SkillManager.Instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
    }
}
