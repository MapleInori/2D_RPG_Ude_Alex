using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallHoldState : PlayerOnWallState
{
    public PlayerWallHoldState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 在墙上不下落
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = 3;
    }

    public override void Update()
    {
        base.Update();
        rb.velocity = new Vector2(0, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
        }

        if (yInput <=0)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        //if (xInput != 0 && player.faceDir != xInput)
        //{
        //    stateMachine.ChangeState(player.idleState);
        //}
        //if (player.IsGroundDetected())
        //{
        //    stateMachine.ChangeState(player.idleState);
        //}
    }
}
