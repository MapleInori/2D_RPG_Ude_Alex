using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ҽű�����Ҫ�����������
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
    // ����״̬��������״̬����
    public PlayerStateMachine stateMachine { get; private set; }
    // ��������״̬�����ں���״̬�л������������״̬�����ܻ������״̬
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
        // ʵ����״̬����״̬
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
        // ״̬�����߼�����
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
        // ��ǽ�ϲ��������𣿻����ǽ��̣��Ҿ������������
        //if (IsWallDetected())
        //    return;

        // ����Dash������CD
        dashUsageTimer -= Time.deltaTime;
        if(dashUsageTimer < 0f)
        {
            dashUsageTimer = 0f;
        }
        // �����乥��ʱ�������̣���غ�������̣���ô�Ķ��о���غ����̳�̵Ĳ����ܽ�Ӳ�����Բ������ơ�
        // ���Ԥ���봦���Ż��˵��ָУ���������һ�°գ���Ȼ������̫���ˡ�
        if (isDropAttacking) return;
        // ��ͳ�ɡ�����������������˵
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer == 0f)
        {
            // ȷ����̷���
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = faceDir;

            dashUsageTimer = dashCoolDown;
            stateMachine.ChangeState(dashState);
        }
    }
    // �������Ž���ʱ��������
    public void AnimationFinishTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }


}
