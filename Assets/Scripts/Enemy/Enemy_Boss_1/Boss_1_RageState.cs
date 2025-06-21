using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss_1_RageState : Boss_1State
{
    public Boss_1_RageState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_1 _boss) : base(_enemyBase, _stateMachine, _animBoolName, _boss)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        Rage();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        // 进入该状态时，boss发生一些变化？比如属性，新增技能等

        // 动画播放完毕
        if (triggerCalled)
        {
            stateMachine.ChangeState(boss.idleState);
        }
    }

    /// <summary>
    /// 狂暴状态，回到起始点播放动画，切换阶段。进入战斗时调用，以及二阶段时调用
    /// </summary>
    public void Rage()
    {
        boss.transform.position = boss.originTrans.position;
        switch (boss.bossPhase)
        {
            case BossPhase.None: boss.bossPhase = BossPhase.First; break;
            case BossPhase.First: boss.bossPhase = BossPhase.Second; break;
            case BossPhase.Second: boss.bossPhase = BossPhase.Third; break;
        }
    }
}
