using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    //TODO： 这个技能不能加CD，如果加了CD，放下水晶后立刻进入CD导致无法瞬移，如果要加CD需要进行适当修改。CD位于Skill中，在这里如果要用，需要额外处理
    // 要加CD应该是完整用完一次后进入CD

    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Crystal Mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInstaedButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private float explisoveCooldown;
    [SerializeField] private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCoolDown;
    [SerializeField] private float useTimeWindow;   // 使用时间窗口，该时间内没有继续使用则进入CD
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();


    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInstaedButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultiStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStack);

    }

    // here we unlock crystal skills
    #region Unlock skill region

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }
    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
        {
            canExplode = true;
            cooldown = explisoveCooldown;
        }
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiStack()
    {
        if (unlockMovingCrystalButton.unlocked)
            canUseMultiStacks = true;
    }

    #endregion     // here we unlock crystal skills


    public override void UseSkill()
    {
        base.UseSkill();
        if (CanUseMultiCrystal())
        {
            return;
        }
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy) return;
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.Instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);

                Destroy(currentCrystal);
            }
            else
            {
                // 交换后立刻爆炸，如果不加这行，那就是到时间爆炸
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.ExplodeOrDestroy();

            }


        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform),player);

    }

    public void CurrentCrystalChooseRandomTarget()
    {
        currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy() ;
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            // 重新生成水晶
            if (crystalLeft.Count > 0)
            {
                // 使用第一个，即开始使用技能，进入内置CD，如果不在该CD内使用完毕，则进入CD
                if (crystalLeft.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }

                // 在能够使用时将CD设置为0，以能够多次使用直到水晶用完
                cooldown = 0;

                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                // 使用后如果水晶数量为0，则重置CD并重新填充水晶
                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCoolDown;
                    RefillCrystal();
                }
                return true;
            }


        }

        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0) return;

        cooldownTimer = multiStackCoolDown;
        RefillCrystal();
    }
}
