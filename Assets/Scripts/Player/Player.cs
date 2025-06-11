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
    public float[] attackMovement;
    public float counterAttackDuration = 0.2f;
    public float baseAttackSpeed = 1f;
    public float extraAttackSpeed = 0f;



    public bool isBusy { get; private set; }
    [Header("Move Info")]
    public float moveSpeed = 7f;
    public float jumpForce = 16f;
    public float wallJumpForce = 6f;
    public float dropAttackForce = 50f;
    public float swordReturnImpact = 16f;
    public bool isDropAttacking;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash Info")]
    public float dashSpeed; 
    public float dashDuration;
    public float dashCoolDown;
    private float defaultDashSpeed;
    [HideInInspector]public float dashUsageTimer;
    [HideInInspector]public float dashDir;


    [HideInInspector]public SkillManager skill;
    public GameObject sword;

    #region States
    // 声明状态机，用于状态控制
    public PlayerStateMachine stateMachine { get; private set; }
    // 声明各种状态，用于后续状态切换，得先有这个状态，才能换到这个状态
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallHoldState wallHoldState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerDropAttackState dropAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackHoleState blackHoleState { get; private set; }
    public PlayerDeadState deadState { get; private set; }

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
}
