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
        // TODO:在移动时受击的一瞬间松手，玩家会保持Idle状态滑行，很奇怪.转向也会被卡住无法转向。
        // DONE, 受击时间导致的，因为受到伤害会有击退效果，而玩家受到高额伤害才会击退，实际运行时没有击退效果时依旧会触发isKnock为true
        // 然后等待击退时间knockDuration，再重置为false，这段时间禁止了移动输入，所以被攻击时会保持上个状态无法切换。
        // 修改为在有实际击退效果时在给击退时间即可，平时设置为0

    }
}
