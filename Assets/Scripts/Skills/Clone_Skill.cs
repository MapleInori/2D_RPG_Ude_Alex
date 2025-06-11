using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 克隆技能
/// </summary>
public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;   // 持续时间
    [SerializeField] private float cloneFadeTime;   // 消失所需时间
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack; // 是否可以攻击

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInseadUnlockButton;
    public bool crystalInsteadOfClone;



    protected override void Start()
    {
        base.Start();


        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInseadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region Unlock region
    //protected override void CheckUnlock()
    //{
    //    UnlockCloneAttack();
    //    UnlockAggresiveClone();
    //    UnlockMultiClone();
    //    UnlockCrystalInstead();
    //}

    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultiClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInseadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }


    #endregion


    public void CreateClone(Transform _cloneTransform, Vector3 _offset)
    {
        if(crystalInsteadOfClone)
        {
            SkillManager.Instance.crystal.CreateCrystal();

            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(
            _cloneTransform, cloneDuration, cloneFadeTime, canAttack, _offset, 
            FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate,player,attackMultiplier);
    }

    public void CreateCloneWithDealy(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector2(1.5f * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _Transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.3f);
        CreateClone(_Transform, _offset);
    }
}
