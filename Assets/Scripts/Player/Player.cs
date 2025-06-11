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
        // ����״̬
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        dropAttackState = new PlayerDropAttackState(this, stateMachine, "DropAttack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        // �ӽ�����״̬
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

        // �ڶ�����
        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        // ��Player�ж�Skill���õ�д����д��Start�ȴ�SkillManagerʵ�������
        StartCoroutine(GetInstance());
        SaveDefaultSpeed();
    }

    /// <summary>
    /// ���泣̬�ٶȣ�Ҳ���������ٶȵ�װ��ʱ���Դ������޸ģ�
    /// </summary>
    private void SaveDefaultSpeed()
    {
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    private IEnumerator GetInstance()
    {
        // �ȴ�һ֡
        yield return null;
        skill = SkillManager.Instance;
    }

    protected override void Update()
    {
        base.Update();
        // ״̬�����߼�����
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

    //// �˳��ڶ�����״̬
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
        // ��ǽ�ϲ��������𣿻����ǽ��̣��Ҿ������������
        //if (IsWallDetected())
        //    return;

        // ����Dash������CD�����ɼ���ʵ�֣��˴�����ʹ��
        //dashUsageTimer -= Time.deltaTime;
        //if(dashUsageTimer < 0f)
        //{
        //    dashUsageTimer = 0f;
        //}
        // �����乥��ʱ�������̣���غ�������̣���ô�Ķ��о���غ����̳�̵Ĳ����ܽ�Ӳ�����Բ������ơ�
        // ���Ԥ���봦���Ż��˵��ָУ���������һ�°գ���Ȼ������̫���ˡ�
        if (isDropAttacking) return;
        // ��ͳ�ɡ�����������������˵
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.Instance.dash.CanUseSkill())
        {
            // ȷ����̷���
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
                dashDir = facingDir;

            //dashUsageTimer = dashCoolDown;
            stateMachine.ChangeState(dashState);
        }
    }
    // �������Ž���ʱ��������
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
