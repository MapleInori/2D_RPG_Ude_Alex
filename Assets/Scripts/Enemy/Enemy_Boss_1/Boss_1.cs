
using UnityEngine;

public enum BossPhase
{
    None,
    First,
    Second,
    Third,
}

public class Boss_1 : Enemy
{

    // 先把原有的行为做出来，然后再看看加什么


    public Transform originTrans;
    public bool openBattle = false;
    public BossPhase bossPhase = BossPhase.None;
    public int phaseHealthThreashold_1_2;   // 1到2阶段血线阈值
    public int phaseHealthThreashold_2_3;   // 2到3阶段血线阈值


    #region 状态机状态

    public Boss_1_IdleState idleState { get; private set; }
    public Boss_1_ShootState shootState { get; private set; }
    public Boss_1_PierceState pierceState { get; private set; }
    public Boss_1_SpinAttackState spinAttackState { get; private set; }
    public Boss_1_RageState rageState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new Boss_1_IdleState(this,stateMachine,"Idle",this);
        shootState = new Boss_1_ShootState(this, stateMachine, "Shoot", this);
        pierceState = new Boss_1_PierceState(this, stateMachine, "Pierce", this);
        spinAttackState = new Boss_1_SpinAttackState(this, stateMachine, "SpinAttack", this);
        rageState = new Boss_1_RageState(this, stateMachine, "Appear", this);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
        transform.position = originTrans.position; // 初始化位置

        phaseHealthThreashold_1_2 = stats.maxHealth.GetValue() * 2 / 3;
        phaseHealthThreashold_2_3 = stats.maxHealth.GetValue() / 3;
    }

    protected override void Update()
    {
        base.Update();
        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    stateMachine.ChangeState(idleState);
        //}
        //else if(Input.GetKeyDown(KeyCode.S))
        //{
        //    stateMachine.ChangeState(shootState);
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    stateMachine.ChangeState(pierceState);
        //}
        //else if (Input.GetKeyDown(KeyCode.F))
        //{
        //    stateMachine.ChangeState(spinAttackState);
        //}
    }

    /// <summary>
    /// 开启boss战
    /// </summary>
    public void StartBattle()
    {
        openBattle = true;
    }


}
