using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;

    private float defaltGravityScale;

    public PlayerBlackHoleState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        skillUsed = false;
        stateTimer = flyTime;
        defaltGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = defaltGravityScale;
        player.fx.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();
        // �ɵ�����֮��
        if (stateTimer <= 0)
        {
            rb.velocity = new Vector2(0, 0);
            if (!skillUsed)
            {
                if (SkillManager.Instance.blackHole.CanUseSkill())
                {
                    skillUsed = true;

                }
            }
            //stateMachine.ChangeState(player.idleState);
        }
        else//�������
        {
            rb.velocity = new Vector2(0, 15);
        }

        // ��BlackHole_Skill_Controller���˳��ڶ�״̬�������޷���֪���ܽ�չ

        // �ʵ��޸�
        if(player.skill.blackHole.SkillCompleted())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
