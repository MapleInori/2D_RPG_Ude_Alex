using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle状态
/// </summary>
public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        // 当横轴有输入时，让状态机切换到移动状态
        // 添加忙碌判断，攻击间隔之间不许移动。算是僵直？
        if (xInput != 0 && !player.isBusy)
        {
            stateMachine.ChangeState(player.moveState);
        }
        // TODO:在移动时受击的一瞬间松手，玩家会保持Idle状态滑行，很奇怪
        if(xInput ==0)
        {
            player.SetZeroVelocity();

        }
    }
}
