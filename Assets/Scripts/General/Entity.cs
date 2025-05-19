using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    // �������������������ڿ��ƶ���
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
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

    public int faceDir { get; private set; } = 1;
    protected bool facingRight = true;


    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {

    }

    public virtual void Damage()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackDirection.x * -faceDir, knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    #region Velocity Methods
    // �����ٶȣ��������ⲿʹ��
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked) return;
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }

    // ���ٶ�
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
        return Physics2D.Raycast(wallCheck.position, Vector2.right * faceDir, wallCheckDistance, groundLayer);
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
    /// ���ҷ�ת��ɫ
    /// </summary>
    public virtual void Flip()
    {
        faceDir = faceDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
    /// <summary>
    /// ��ת�����������ڿ��ƽ�ɫ�ķ�ת
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
}
