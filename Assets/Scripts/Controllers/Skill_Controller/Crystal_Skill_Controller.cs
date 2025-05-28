using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水晶技能控制器，有存在时间，所以如果没追上敌人会提前爆炸
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
    /// 能爆炸则爆炸，不能爆炸则销毁自身
    /// </summary>
    public void ExplodeOrDestroy()
    {
        if (canExplode)
        {
            //TODO:这个爆炸逻辑怪怪的，当开始爆炸的时候放大，但是很显然没有放大完的时候就炸了，动画结束后销毁自身，因为动画并不会等放大结束？
            // 如果制作更合理的逻辑？
            // 这样是爆炸的时候有一个放大的效果，间接实现了爆炸时特效逐渐放大，但是无法确定最后放大到多大了，如何确保最后销毁前放大到目标大小呢？
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
    /// TODO：待扩展
    /// 想给所有计时器都有一个这样的方法，因为每个计时器都想预防溢出，就会反复写很多遍这段代码，只是换个变量
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
