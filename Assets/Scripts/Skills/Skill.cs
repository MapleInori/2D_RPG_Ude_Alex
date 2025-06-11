using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ܻ���
/// </summary>
public class Skill : MonoBehaviour
{
    public float cooldown;
    protected float cooldownTimer;
    protected Player player;

    protected virtual void Awake()
    {
        // TODO:���ܻ�ȡ����
    }

    private IEnumerator GetInstance()
    {
        yield return null;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerManager.Instance.player;

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
    /// ��ʹ����ʹ��
    /// </summary>
    /// <returns>����trueʱʹ�ü��ܲ�����CD������falseʱû��ʹ�ü���</returns>
    public virtual bool CanUseSkill()
    {
        if(cooldownTimer <=0)
        {
            // ʹ�ü���
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        return false;
    }

    /// <summary>
    /// ���ܵľ���ʵ��
    /// </summary>
    public virtual void UseSkill()
    {

    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 15);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // �ҵ�����ĵ���
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
