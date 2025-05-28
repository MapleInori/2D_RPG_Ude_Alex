using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ˮ�����ܿ��������д���ʱ�䣬�������û׷�ϵ��˻���ǰ��ը
/// </summary>
public class Crystal_Skill_Controller : MonoBehaviour
{
    private Player player;
    private Animator anim;
    private CircleCollider2D circleCollider;
    public float crystalExistTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    public Transform closestTarget;
    [SerializeField] private LayerMask enemyLayer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget,Player _player)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
        player = _player;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.Instance.blackHole.GetBlackHoleRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        if (colliders.Length > 0)
        {
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;

        }
    }

    private void Update()
    {
        TimeMinus();
        if (crystalExistTimer <= 0)
        {
            ExplodeOrDestroy();
        }

        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 0.5f)
            {
                ExplodeOrDestroy();
                canMove = false;
            }
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector3(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void ExplodeAnimationEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }
    /// <summary>
    /// �ܱ�ը��ը�����ܱ�ը����������
    /// </summary>
    public void ExplodeOrDestroy()
    {
        if (canExplode)
        {
            //TODO:�����ը�߼��ֵֹģ�����ʼ��ը��ʱ��Ŵ󣬵��Ǻ���Ȼû�зŴ����ʱ���ը�ˣ���������������������Ϊ����������ȷŴ������
            // ���������������߼���
            // �����Ǳ�ը��ʱ����һ���Ŵ��Ч�������ʵ���˱�ըʱ��Ч�𽥷Ŵ󣬵����޷�ȷ�����Ŵ󵽶���ˣ����ȷ���������ǰ�Ŵ�Ŀ���С�أ�
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();

        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// TODO������չ
    /// ������м�ʱ������һ�������ķ�������Ϊÿ����ʱ������Ԥ��������ͻᷴ��д�ܶ����δ��룬ֻ�ǻ�������
    /// </summary>
    private void TimeMinus()
    {
        if (crystalExistTimer > 0)
        {
            crystalExistTimer -= Time.deltaTime;
        }
        else
        {
            crystalExistTimer = 0;
        }
    }
}
