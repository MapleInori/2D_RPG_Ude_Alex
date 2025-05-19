using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动状态
/// </summary>
public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);
        // 当横轴没有输入时，让状态机切换到闲置状态
        if (xInput == 0 )
        {
            player.SetZeroVelocity();
            stateMachine.ChangeState(player.idleState);
        }
        // 凭什么不能对着墙撞，我觉得可以
        //if(player.IsWallDetected())
        //{
        //    stateMachine.ChangeState(player.idleState);
        //}
    }
}
