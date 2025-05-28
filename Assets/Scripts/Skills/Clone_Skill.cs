using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 克隆技能
/// </summary>
public class Clone_Skill : Skill
{
    [SerializeField] private bool canAttack; // 是否可以攻击
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;   // 持续时间
    [SerializeField] private float cloneFadeTime;   // 消失所需时间


    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool canCreateCloneOnCounterAttack;
    [Header("Clone can Duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
    public bool crystalInsteadOfClone;


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
            FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate,player);
    }

    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
        {
            CreateClone(player.transform, Vector2.zero);
        }
    }

    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
        {
            CreateClone(player.transform, Vector2.zero);
        }
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
        {
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector2(1.5f * player.faceDir, 0)));
        }
    }

    private IEnumerator CreateCloneWithDelay(Transform _Transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.3f);
        CreateClone(_Transform, _offset);
    }
}
