using System.Collections;
using UnityEngine;

// 敌人类，继承自Entity基类，实现敌人特有的行为逻辑
public class Enemy : Entity
{
    [SerializeField] protected LayerMask playerLayer;   // 玩家层级，用于检测玩家

    [Header("Stunned Info")] 
    public float stunDuration;  // 眩晕持续时间
    public Vector2 stunDirection;   // 眩晕时的击退方向
    protected bool canBeStunned;    // 当前是否可被眩晕（处于反击窗口期）
    [SerializeField] protected GameObject counterImage; // 反击提示UI图标

    [Header("Move Info")]
    public float moveSpeed;          // 移动速度
    public float idleTime;           // 空闲状态持续时间
    public float battleTime;         // 战斗状态持续时间
    public float checkDistance;      // 玩家检测距离
    public float checkRadius;        // 玩家检测范围半径
    public float defaultMoveSpeed;   // 默认移动速度（用于重置）

    [Header("Attack Info")]
    public float attackDistance;     // 攻击触发距离
    public float attackCoolDown;     // 攻击冷却时间
    public float minAttackCooldown = 1; // 最小攻击冷却
    public float maxAttackCooldown = 2; // 最大攻击冷却
    public float lastTimeAttacked;   // 上次攻击时间戳

    // 敌人状态机（管理空闲/移动/攻击等状态）
    public EnemyStateMachine stateMachine { get; private set; }
    // 记录上一个动画状态名称（用于动画过渡）
    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine(); // 创建状态机实例
        defaultMoveSpeed = moveSpeed;    // 保存默认速度

    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update(); // 执行当前状态的更新逻辑
    }
    // 实现减速效果（重写父类方法）
    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);  // 降低移动速度
        anim.speed = anim.speed * (1 - _slowPercentage);    // 降低动画速度
        // 设置定时恢复默认速度.Note：如果反复在第一次恢复前又触发一次，会按最后一次的慢速持续时间来恢复吗？
        Invoke("ReturnDefaultSpeed", _slowDuration);
    }
    // 恢复默认速度
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;   // 重置移动速度

    }
    // 记录最后播放的动画名称（用于状态过渡）
    public virtual void AssignLastAnimName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;
    }
    // 时间冻结控制（如技能效果）
    public virtual void FreezeTime(bool _timeFrozen)
    {
        if(_timeFrozen) // 停止移动
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else // 恢复移动
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }
    // 临时冻结时间协程
    public virtual void FreezeTimeFor(float _duration)
    {
        StartCoroutine(FreezeTimeCoroutine(_duration));
    }
    // 时间冻结协程实现
    protected virtual IEnumerator FreezeTimeCoroutine(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
    }

    #region CounterAttackWindow
    // 开启反击窗口（玩家可触发反击）
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }
    // 关闭反击窗口
    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion
    /// <summary>
    /// 检查是否可被眩晕（当玩家成功反击时调用）
    /// </summary>
    public virtual bool CanBeStunned()
    {
        if(canBeStunned)// 如果处于反击窗口期
        {
            CloseCounterAttackWindow(); // 关闭窗口
            return true;    // 返回可眩晕
        }
        else
        {
            return false;   // 否则返回不可眩晕
        }
    }
    // 动画结束回调（通知状态机）
    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    // 玩家检测：朝当前面向方向发射射线
    public virtual RaycastHit2D isPlayerDetected() => Physics2D.Raycast(wallCheck.position,Vector2.right * facingDir, checkDistance, playerLayer);
    // 调试绘制（扩展父类方法）
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();// 绘制父类的碰撞检测

        // 额外绘制攻击距离指示线
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(attackDistance * facingDir, 0));
    }

}
