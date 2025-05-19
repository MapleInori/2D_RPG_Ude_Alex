using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ƶ�״̬
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
        // ������û������ʱ����״̬���л�������״̬
        if (xInput == 0 )
        {
            player.SetZeroVelocity();
            stateMachine.ChangeState(player.idleState);
        }
        // ƾʲô���ܶ���ǽײ���Ҿ��ÿ���
        //if(player.IsWallDetected())
        //{
        //    stateMachine.ChangeState(player.idleState);
        //}
    }
}
