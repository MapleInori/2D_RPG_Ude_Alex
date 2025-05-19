using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropAttackState : PlayerState
{
    private int animPhase;
    public PlayerDropAttackState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isDropAttacking = true;
        animPhase = 0;
        // ���õڶ��ε������ζ�������
        player.anim.SetBool("DropAttackAtGround", false);
    }

    public override void Exit()
    {
        base.Exit();
        player.isDropAttacking = false;
    }

    public override void Update()
    {
        base.Update();
        // ǰ���߼��������߼����л�
        if (animPhase == 0 && triggerCalled)
        {
            animPhase = 1;
            triggerCalled = false;
        }
        // �����߼��������߼����л�
        if(animPhase == 1 && player.IsGroundDetected())
        {
            animPhase = 2;
            triggerCalled = false;
        }


        // ��һ�Σ�ǰ�ö����������ֵ���ǰһ֡����triggerCalledΪtrue
        // ��һ�β������ֱ�ӽ���ڶ��ζ���
        if (animPhase==0)
            rb.velocity = new Vector2(0, 0);

        // ���ֵ�����������
        // �ڶ������䶯����ѭ������
        if (!player.IsGroundDetected() && animPhase == 1)
        {
            rb.velocity = new Vector2(0,player.dropAttackForce) * Vector2.down;
        }

        // �ڶ������۲��ŵ����ֻҪ�������,���ǵ�һ��һ��Ҫ������ģ����Ե�һ�ε�exit timeΪ1
        if(player.IsGroundDetected())
        {
            // Ԥ�������л����
            if(dashPreInput && player.dashUsageTimer ==0f)
            {
                stateMachine.ChangeState(player.dashState);
                return;
            }
            player.anim.SetBool("DropAttackAtGround", true);
            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }

        // ����и��Ӹ��ӵ����乥��Ч����Ӧ����Ҫ�������״̬�����׿���
    }
}
