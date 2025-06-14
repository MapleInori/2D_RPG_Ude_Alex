using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能基类
/// </summary>
public class Skill : MonoBehaviour
{
    public float cooldown;
    public float cooldownTimer;
    protected Player player;

    protected virtual void Awake()
    {
        // TODO:可能获取不到
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerManager.Instance.player;
        // TODO：可能无法读档解锁，因为它的执行在读档之前，导致读档时这里提前检查完了，CNMLGB，又是执行顺序问题，读档的顺序怎么这么靠后？
        //CheckUnlock();
        Invoke("CheckUnlock",1);

    }

    protected virtual void CheckUnlock()
    {

    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (cooldownTimer >= 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    /// <summary>
    /// 能使用则使用
    /// </summary>
    /// <returns>返回true时使用技能并进入CD，返回false时没有使用技能</returns>
    public virtual bool CanUseSkill()
    {
        if(cooldownTimer <=0)
        {
            // 使用技能
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 技能的具体实现
    /// </summary>
    public virtual void UseSkill()
    {

    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 15);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // 找到最近的敌人
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distance = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }
}
