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
    [SerializeField] private Vector2 knockbackDirection;
    [SerializeField] private float knockbackDuration;
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

    }

    public virtual void SlowEntityBy(float _slowPercentage,float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }
    /// <summary>
    /// 受伤效果
    /// </summary>
    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback");
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * -facingDir, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
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

    }

}
