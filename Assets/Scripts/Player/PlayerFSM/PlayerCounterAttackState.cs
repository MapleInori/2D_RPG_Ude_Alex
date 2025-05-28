using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        canCreateClone = true;
        // ����״̬����ʱ��
        stateTimer = player.counterAttackDuration;
        // ���÷����ɹ��ж�
        player.anim.SetBool("SuccessfulCounterAttack", false);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        // TODO������ʱӦ�����޵е�
        player.SetZeroVelocity();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.attackCheck.transform.position, player.attackCheckRadius);

        foreach (var hit in hitColliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    //TODO: ��������ʱ��ľ���������ʲô��������ӻ�������
                    stateTimer = 5f;
                    player.anim.SetBool("SuccessfulCounterAttack", true);

                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.clone.CreateCloneOnCounterAttack(hit.transform);
                    }
                }
            }
        }
        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }


    }
}
