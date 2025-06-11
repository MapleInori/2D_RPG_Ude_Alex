using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
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
        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackHole.blackholeUnlocked)
        {
            stateMachine.ChangeState(player.blackHoleState);
        }
        if(Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
        {
            stateMachine.ChangeState(player.counterAttackState);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }

        // 如果没在地面，则切换到空中状态
        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

}
