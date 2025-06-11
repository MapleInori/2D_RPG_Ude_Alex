using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHole_Skill : Skill
{

    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackholeUnlocked;// { get; private set; }
    [SerializeField] private int amountOfAttack;
    [SerializeField] private float cloneAttackCoolDown ;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float blackHoleDuration;

    BlackHole_Skill_Controller currentBlackHole;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);

        currentBlackHole = newBlackHole.GetComponent<BlackHole_Skill_Controller>();

        currentBlackHole.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttack, cloneAttackCoolDown,blackHoleDuration);
    }

    protected override void Start()
    {
        base.Start();

        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }
    private void UnlockBlackhole()
    {
        if (blackHoleUnlockButton.unlocked)
            blackholeUnlocked = true;

    }


    public bool SkillCompleted()
    {
        if(!currentBlackHole)
        {
            return false;
        }

        if(currentBlackHole.playerCanExitState)
        {
            currentBlackHole = null;
            return true;
        }

        return false;
    }

    public float GetBlackHoleRadius()
    {
        // 除以2是对应碰撞体半径为图像大小的一半
        return maxSize / 2;
    }
}
