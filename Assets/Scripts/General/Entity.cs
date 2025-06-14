using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    #region Components
    // 声明动画控制器，用于控制动画
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D capsuleCollider { get; private set; }
    #endregion

    [Header("Knockback Info")]
    [SerializeField] protected Vector2 knockbackPower;    // 受击后被击退的力量，将玩家设置为0可以避免玩家被击退
    [SerializeField] public float knockbackDuration;
    protected bool isKnocked;


    [Header("Collision Info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask groundLayer;

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public UnityAction onFliped;
    private int knockbackDir;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }
    }

    public virtual void SlowEntityBy(float _slowPercentage,float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }
    /// <summary>
    /// 受伤后效果，在伤害计算后
    /// </summary>
    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;


    }
    /// <summary>
    /// 设置击退力量，在伤害计算前
    /// </summary>
    /// <param name="_knockbackpower"></param>
    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        if (knockbackPower.x > 0 || knockbackPower.y > 0) // This line makes player immune to freeze effect when he takes hit
            rb.velocity = new Vector2((knockbackPower.x ) * knockbackDir, knockbackPower.y);
        // 即使玩家没有被击退，这里会导致持续期间isKnocked为true无法移动，以及这期间松开移动按键会保持滑行，
        // 因为进入Idle时，调用的SetZeroVelocity由于检测到被击退没有设置速度，所以玩家不该有初始knockbackDuration，应该在使用时设置，使用后重置为0
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #region Velocity Methods
    // 设置速度，公开给外部使用
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked) return;
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    // 零速度
    public void SetZeroVelocity()
    {
        if (isKnocked) return;
        rb.velocity = new Vector2(0, 0);
    }

    #endregion

    #region Some methods of Collision
    public virtual bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public virtual bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, groundLayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance, 0));
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(wallCheckDistance, 0, 0));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip Control
    /// <summary>
    /// 左右翻转角色
    /// </summary>
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFliped?.Invoke();
    }
    /// <summary>
    /// 翻转控制器，用于控制角色的翻转
    /// </summary>
    protected virtual void FlipController(float _xVelocity)
    {
        if (_xVelocity > 0 && !facingRight)
        {
            Flip();
        }
        else if (_xVelocity < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion


    public virtual void Die()
    {
        Destroy(gameObject, 5f);
    }

}
