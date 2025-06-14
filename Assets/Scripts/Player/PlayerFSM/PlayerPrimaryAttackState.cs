using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private float attackWindow = 1.5f;
    private float lastAttackTime;
    public int comboCounter { get; private set; }
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine stateMachine, string _animBoolName) : base(_player, stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //AudioManager.instance.PlaySFX(2,player.transform);
        // Time.time全局变量，所以只需要依靠最后攻击时间和攻击窗口期即可判断连击，而不需要计时器
        if (comboCounter > 2 || Time.time > lastAttackTime + attackWindow)
        {
            comboCounter = 0;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        // 切换攻击方向，因为根据速度方向与面向方向自动控制翻转，而攻击时会有攻击方向上的速度，所以这里不需要额外处理翻转
        float attackDir = player.facingDir;
        if(xInput != 0)
        {
            attackDir = xInput;
        }

        
        // 攻击的移动偏移
        player.SetVelocity(player.attackMovement[comboCounter] * attackDir, rb.velocity.y);
        // 攻击速度可以修改，增加动画速度即可，退出时重置
        player.anim.speed = player.baseAttackSpeed + player.extraAttackSpeed;
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
        // 添加0.15f的攻击后摇，不许移动。但其实可以用dash打断，冲刺时间大于这个时间
        player.StartCoroutine("BusyFor", 0.15f);
        comboCounter++;
        lastAttackTime = Time.time;
        // 动画速度重置
        player.anim.speed = player.baseAttackSpeed;
    }

    public override void Update()
    {
        base.Update();

        // 原本移动的情况下，0.1f后停下来
        if(stateTimer <0)
        {
            player.SetZeroVelocity();
        }
        // 动画结束时切换回idle，从idle再回到第二段攻击
        if(triggerCalled)
        {
            // 预输入再切换冲刺
            if (dashPreInput && player.dashUsageTimer == 0f)
            {
                stateMachine.ChangeState(player.dashState);
                return;
            }
            stateMachine.ChangeState(player.idleState);
        }
    }
}
