using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家脚本，主要用于总体管理。
/// </summary>
public class Player : Entity
{

    [Header("Attack Info")]
    public float[] attackMovement;          // 不同攻击动作的前冲力量数组
    public float counterAttackDuration = 0.2f; // 反击窗口持续时间
    public float baseAttackSpeed = 1f;      // 基础攻击速度
    public float extraAttackSpeed = 0f;     // 额外攻击速度加成

    public bool isBusy { get; private set; } // 标记玩家是否处于忙碌状态（无法响应输入）

    [Header("Move Info")]
    public float moveSpeed = 7f;            // 基础移动速度
    public float jumpForce = 16f;           // 普通跳跃力量
    public float wallJumpForce = 6f;        // 蹬墙跳力量
    public float dropAttackForce = 50f;     // 下落攻击力量
    public float swordReturnImpact = 16f;   // 接剑时的冲击力
    public bool isDropAttacking;            // 标记是否正在下落攻击
    private float defaultMoveSpeed;         // 默认移动速度（用于重置）
    private float defaultJumpForce;         // 默认跳跃力量（用于重置）

    [Header("Dash Info")]
    public float dashSpeed;                 // 冲刺速度
    public float dashDuration;              // 冲刺持续时间
    public float dashCoolDown;              // 冲刺冷却时间
    private float defaultDashSpeed;         // 默认冲刺速度（用于重置）
    [HideInInspector] public float dashUsageTimer; // 冲刺使用计时器
    [HideInInspector] public float dashDir;       // 冲刺方向

    [HideInInspector] public SkillManager skill; // 技能管理器引用
    public GameObject sword;                // 玩家持有的剑对象


    #region States
    public PlayerStateMachine stateMachine { get; private set; }                // 声明状态机，用于状态控制    
    // 声明各种状态，用于后续状态切换，得先有这个状态，才能换到这个状态
    public PlayerIdleState idleState { get; private set; }                      // 空闲状态
    public PlayerMoveState moveState { get; private set; }                      // 移动状态
    public PlayerAirState airState { get; private set; }                        // 空中状态
    public PlayerJumpState jumpState { get; private set; }                      // 跳跃状态
    public PlayerDashState dashState { get; private set; }                      // 冲刺状态
    public PlayerWallSlideState wallSlideState { get; private set; }            // 滑墙状态
    public PlayerWallHoldState wallHoldState { get; private set; }              // 贴墙状态
    public PlayerWallJumpState wallJumpState { get; private set; }              // 蹬墙跳状态
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }    // 主要攻击状态
    public PlayerDropAttackState dropAttackState { get; private set; }          // 下落攻击状态
    public PlayerCounterAttackState counterAttackState { get; private set; }    // 反击状态
    public PlayerAimSwordState aimSwordState { get; private set; }              // 瞄准扔剑状态
    public PlayerCatchSwordState catchSwordState { get; private set; }          // 接剑状态
    public PlayerBlackHoleState blackHoleState { get; private set; }            // 黑洞技能状态
    public PlayerDeadState deadState { get; private set; }                      // 死亡状态


    #endregion

    protected override void Awake()
    {
        base.Awake();
        isBusy = false;
        // 实例化状态机和状态
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this,stateMachine,"Idle");
        moveState = new PlayerMoveState(this,stateMachine,"Move");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this,stateMachine, "WallSlide");
        wallHoldState = new PlayerWallHoldState(this, stateMachine, "WallSlideIdle");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        // 攻击状态
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        dropAttackState = new PlayerDropAttackState(this, stateMachine, "DropAttack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        // 扔剑技能状态
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

        // 黑洞技能
        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        // 简化Player中对Skill调用的写法，写在Start等待SkillManager实例化完成
        // 异步获取技能管理器实例
        StartCoroutine(GetInstance());
        SaveDefaultSpeed();
    }

    /// <summary>
    /// 保存常态速度，也许有增加速度的装备时可以从这里修改？
    /// </summary>
    private void SaveDefaultSpeed()
    {
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    private IEnumerator GetInstance()
    {
        // 等待一帧
        yield return null;
        skill = SkillManager.Instance;
    }

    protected override void Update()
    {
        base.Update();
        // 状态机的逻辑更新
        stateMachine.currentState.Update();
        CheckForDashInput();

        if(Input.GetKeyDown(KeyCode.I) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.Instance.UseFlask();
        }
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed",_slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public void AssignNewSword(GameObject newSword)
    {
        sword = newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    //// 退出黑洞技能状态
    //public void ExitBlackHoleAbility()
    //{
    //    stateMachine.ChangeState(airState);
    //}

    public IEnumerator BusyFor(float _second)
    {
        isBusy = true;
        yield return new WaitForSeconds(_second);
        isBusy = false;
    }

    public void CheckForDashInput()
    {
        // TODO：重新加载场景时，skill可能没有加载完成导致空引用NullReferenceException: Object reference not set to an instance of an object
        // 这种问题也太常见了....
        if (skill == null)
        {
            if(skill.dash == null)
            {
                Debug.Log("Skill.dash is null");
                return;
            }
            Debug.Log("Skill is null");
            return;
        }
        if (skill.dash.dashUnlocked == false)
            return;
        // 在墙上不允许冲刺吗？会对着墙冲刺，我觉得这是允许的
        //if (IsWallDetected())
        //    return;

        // 限制Dash，增加CD。已由技能实现，此处不再使用
        //dashUsageTimer -= Time.deltaTime;
        //if(dashUsageTimer < 0f)
        //{
        //    dashUsageTimer = 0f;
        //}
        // 当下落攻击时不允许冲刺，落地后才允许冲刺，怎么改都感觉落地后立刻冲刺的操作很僵硬，索性不再限制。
        // 添加预输入处理，优化了点手感，还是限制一下罢，不然看起来太怪了。
        if (isDropAttacking) return;
        // 你就冲吧。————阿杰如是说
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.dash.CanUseSkill())
        {
            // 确定冲刺方向
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = facingDir;

            //dashUsageTimer = dashCoolDown;
            stateMachine.ChangeState(dashState);
        }
    }
    // 动画播放结束时触发调用
    public void AnimationFinishTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
    /// <summary>
    /// 在受到伤害后调用，清空玩家被击退力量，避免普通伤害也造成击退
    /// </summary>
    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
        knockbackDuration = 0f;
    }
}
