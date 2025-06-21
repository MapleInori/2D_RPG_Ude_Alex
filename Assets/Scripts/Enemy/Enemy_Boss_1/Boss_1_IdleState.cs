using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1_IdleState : Boss_1State
{

    public Boss_1_IdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_1 _boss) : base(_enemyBase, _stateMachine, _animBoolName, _boss)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
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

        // 初次进入战斗时，以及血量到阈值时切换阶段
        ChangePhase();

        // 什么时候射击

        // 什么时候穿刺

        // 什么时候旋转攻击

        // 什么时候其他技能

    }

    private void ChangePhase()
    {
        if (boss.openBattle && boss.bossPhase == BossPhase.None)     // 进入第一阶段
        {
            stateMachine.ChangeState(boss.rageState);
        }
        else if (boss.stats.currentHealth < boss.phaseHealthThreashold_1_2 && boss.bossPhase == BossPhase.First)     // 切换到第二阶段
        {
            stateMachine.ChangeState(boss.rageState);
        }
        else if (boss.stats.currentHealth < boss.phaseHealthThreashold_2_3 && boss.bossPhase == BossPhase.Second)    // 切换到第三阶段
        {
            stateMachine.ChangeState(boss.rageState);
        }
    }
}
