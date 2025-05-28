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
        // 反击状态持续时间
        stateTimer = player.counterAttackDuration;
        // 重置反击成功判断
        player.anim.SetBool("SuccessfulCounterAttack", false);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        // TODO：反击时应该是无敌的
        player.SetZeroVelocity();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.attackCheck.transform.position, player.attackCheckRadius);

        foreach (var hit in hitColliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    //TODO: 这里重置时间的具体意义是什么？如果不加会怎样？
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
