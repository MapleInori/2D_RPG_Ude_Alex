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

    public float baseAttackSpeed = 1f;
    public float extraAttackSpeed = 0f;



    public bool isBusy { get; private set; }
    [Header("Move Info")]
    public float moveSpeed = 7f;
    public float jumpForce = 16f;
    public float wallJumpForce = 6f;
    public float dropAttackForce = 50f;
    public bool isDropAttacking;

    [Header("Dash Info")]
    public float dashSpeed; 
    public float dashDuration;
    public float dashCoolDown;
    [HideInInspector]public float dashUsageTimer;
    [HideInInspector]public float dashDir;




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
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        dropAttackState = new PlayerDropAttackState(this, stateMachine, "DropAttack");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        // 状态机的逻辑更新
        stateMachine.currentState.Update();
        CheckForDashInput();
    }

    

    public IEnumerator BusyFor(float _second)
    {
        isBusy = true;
        yield return new WaitForSeconds(_second);
        isBusy = false;
    }

    public void CheckForDashInput()
    {
        // 在墙上不允许冲刺吗？会对着墙冲刺，我觉得这是允许的
        //if (IsWallDetected())
        //    return;

        // 限制Dash，增加CD
        dashUsageTimer -= Time.deltaTime;
        if(dashUsageTimer < 0f)
        {
            dashUsageTimer = 0f;
        }
        // 当下落攻击时不允许冲刺，落地后才允许冲刺，怎么改都感觉落地后立刻冲刺的操作很僵硬，索性不再限制。
        // 添加预输入处理，优化了点手感，还是限制一下罢，不然看起来太怪了。
        if (isDropAttacking) return;
        // 你就冲吧。————阿杰如是说
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer == 0f)
        {
            // 确定冲刺方向
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = faceDir;

            dashUsageTimer = dashCoolDown;
            stateMachine.ChangeState(dashState);
        }
    }
    // 动画播放结束时触发调用
    public void AnimationFinishTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }


}
