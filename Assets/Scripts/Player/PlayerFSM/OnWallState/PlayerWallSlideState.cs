using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerOnWallState
{
    public float wallSlideSpeedRate = 0.2f;

    public PlayerWallSlideState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        // 在墙上时，如果有垂直输入
        if (yInput <0)
        {
            player.SetVelocity(0, rb.velocity.y);
        }
        else if(yInput ==0)
        {
            player.SetVelocity(0, rb.velocity.y * wallSlideSpeedRate);
        }
        else
        {
            stateMachine.ChangeState(player.wallHoldState);
        }


    }
}
