using System.Collections;
using UnityEngine;
using UnityEngine.Events;
// Entity类：所有游戏实体（玩家、敌人等）的基类，处理通用物理、动画和战斗逻辑
public class Entity : MonoBehaviour
{
    #region Components
    // 声明动画控制器，用于控制动画
    public Animator anim { get; private set; }      // 动画控制器组件
    public Rigidbody2D rb { get; private set; }     // 刚体物理组件
    public EntityFX fx { get; private set; }        // 实体特效组件（如受击闪烁）
    public SpriteRenderer sr { get; private set; }   // 精灵渲染器（控制显示）
    public CharacterStats stats { get; private set; } // 角色属性组件（生命值/攻击力等）
    public CapsuleCollider2D capsuleCollider { get; private set; } // 碰撞体组件
    #endregion

    [Header("Knockback Info")]
    [SerializeField] protected Vector2 knockbackPower;    // 受击后被击退的力量，将玩家设置为0可以避免玩家被击退(X:水平击退力, Y:垂直击退力)
    [SerializeField] public float knockbackDuration;    // 击退持续时间
    protected bool isKnocked;   // 当前是否处于击退状态


    [Header("Collision Info")]
    public Transform attackCheck;             // 攻击判定点位置
    public float attackCheckRadius;           // 攻击检测半径
    [SerializeField] protected Transform groundCheck;   // 地面检测点
    [SerializeField] protected float groundCheckDistance; // 地面检测距离
    [SerializeField] protected Transform wallCheck;      // 墙壁检测点
    [SerializeField] protected float wallCheckDistance;  // 墙壁检测距离
    [SerializeField] protected LayerMask groundLayer;    // 地面层级掩码

    public int facingDir { get; private set; } = 1; // 当前面向方向 (1:右, -1:左)
    protected bool facingRight = true;  // 是否面朝右侧

    public UnityAction onFliped;    // 翻转事件回调
    private int knockbackDir;   // 击退方向 (根据伤害来源确定)

    // 初始化组件（空方法，供子类扩展）
    protected virtual void Awake() {}
    // 组件初始化：获取关键组件引用
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
        if(Time.timeScale == 0) // 当游戏暂停时跳过更新
        {
            return;
        }
    }
    // 实体减速效果（需子类实现具体逻辑）
    public virtual void SlowEntityBy(float _slowPercentage,float _slowDuration) {}
    // 恢复默认移动速度（重置动画速度）
    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1; // 恢复动画正常播放速度
    }
    /// <summary>
    /// 受伤后效果（在伤害计算后调用）
    /// </summary>
    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockback"); // 触发击退协程
    }
    // 设置击退方向（基于伤害来源位置）
    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        // 根据伤害源位置确定击退方向（左/右）
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;  // 伤害源在右侧 → 向左击退
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;   // 伤害源在左侧 → 向右击退 


    }
    /// <summary>
    /// 动态设置击退力量（在伤害计算前调用）
    /// </summary>
    /// <param name="_knockbackpower"></param>
    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;
    // 击退效果协程
    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;   // 标记为击退状态
        // 应用击退速度（仅在击退力量有效时）
        if (knockbackPower.x > 0 || knockbackPower.y > 0) // This line makes player immune to freeze effect when he takes hit
            rb.velocity = new Vector2((knockbackPower.x ) * knockbackDir, knockbackPower.y);
        // 即使玩家没有被击退，这里会导致持续期间isKnocked为true无法移动，以及这期间松开移动按键会保持滑行，
        // 因为进入Idle时，调用的SetZeroVelocity由于检测到被击退没有设置速度，所以玩家不该有初始knockbackDuration，应该在使用时设置，使用后重置为0
        yield return new WaitForSeconds(knockbackDuration); // 等待击退持续时间
        isKnocked = false;  // 结束击退状态
        SetupZeroKnockbackPower();  // 重置击退参数
    }
    // 重置击退力量（供子类扩展）
    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #region Velocity Methods
    // 设置实体速度（自动处理面向方向）
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked) return;  // 击退状态下禁止控制
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity); // 根据速度方向自动翻转
    }

    // 停止实体移动
    public void SetZeroVelocity()
    {
        if (isKnocked) return;  // 击退状态下保持原有速度
        rb.velocity = new Vector2(0, 0);    // 速度归零
    }

    #endregion

    #region Some methods of Collision
    // 地面检测：向下发射射线检测地面
    public virtual bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
    }
    // 墙壁检测：朝当前面向方向发射射线检测墙壁
    public virtual bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, groundLayer);
    }
    // 调试绘制：在Scene视图显示碰撞检测范围
    protected virtual void OnDrawGizmos()
    {
        // 绘制地面检测线（从检测点向下）
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance, 0));
        // 绘制墙壁检测线（沿当前面向方向）
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(wallCheckDistance, 0, 0));
        // 绘制攻击检测范围（球形区域）
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip Control
    /// <summary>
    /// 左右翻转角色
    /// </summary>
    public virtual void Flip()
    {
        facingDir = facingDir * -1;  // 切换方向值
        facingRight = !facingRight; // 更新面向标志
        transform.Rotate(0, 180, 0);    // 实际旋转游戏对象

        onFliped?.Invoke(); // 触发翻转事件（通知监听者）
    }
    /// <summary>
    /// 翻转控制器，用于控制角色的翻转
    /// </summary>
    protected virtual void FlipController(float _xVelocity)
    {
        // 向右移动且当前朝左 → 翻转
        if (_xVelocity > 0 && !facingRight)
        {
            Flip();
        }
        else if (_xVelocity < 0 && facingRight) // 向左移动且当前朝右 → 翻转
        {
            Flip();
        }
    }
    #endregion

    // 实体死亡处理（基础实现：5秒后销毁对象）
    public virtual void Die()
    {
        Destroy(gameObject, 5f);    // 延迟销毁实体对象
    }

}
