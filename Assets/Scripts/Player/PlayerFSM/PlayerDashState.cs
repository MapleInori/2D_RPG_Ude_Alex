

public class PlayerDashState : PlayerState
{
    private float dashSpeed;
    public PlayerDashState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        dashSpeed = player.dashSpeed;
        stateTimer = player.dashDuration;

        player.skill.dash.CreateCloneOnDashStart();
    }

    public override void Exit()
    {
        base.Exit();
        player.skill.dash.CreateCloneOnDashOver();
        // ȡ�����״̬ʱ�������ٶ�Ϊ0
        player.SetVelocity(0, rb.velocity.y);

    }

    public override void Update()
    {
        base.Update();

        // ���ʱ������
        player.SetVelocity(dashSpeed * player.dashDir, 0);
        if(stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
