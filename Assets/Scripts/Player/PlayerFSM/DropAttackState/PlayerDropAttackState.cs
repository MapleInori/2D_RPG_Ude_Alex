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
        // 重置第二段到第三段动画条件
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
        // 前置逻辑到下落逻辑的切换
        if (animPhase == 0 && triggerCalled)
        {
            animPhase = 1;
            triggerCalled = false;
        }
        // 下落逻辑到结束逻辑的切换
        if(animPhase == 1 && player.IsGroundDetected())
        {
            animPhase = 2;
            triggerCalled = false;
        }


        // 第一段（前置动作），出现刀光前一帧设置triggerCalled为true
        // 第一段播放完会直接进入第二段动画
        if (animPhase==0)
            rb.velocity = new Vector2(0, 0);

        // 出现刀光后加速下落
        // 第二段下落动画会循环播放
        if (!player.IsGroundDetected() && animPhase == 1)
        {
            rb.velocity = new Vector2(0,player.dropAttackForce) * Vector2.down;
        }

        // 第二段无论播放到哪里，只要到达地面,但是第一段一定要播放完的，所以第一段的exit time为1
        if(player.IsGroundDetected())
        {
            // 预输入再切换冲刺
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

        // 如果有更加复杂的下落攻击效果，应该需要拆成三个状态更容易控制
    }
}
