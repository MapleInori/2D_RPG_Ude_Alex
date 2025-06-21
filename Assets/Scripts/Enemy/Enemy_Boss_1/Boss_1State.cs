using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1State : EnemyState
{
    protected Boss_1 boss;
    private bool isSleep = true;
    public Boss_1State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Boss_1 _boss) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        boss = _boss;
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
        // 从非战斗状态进入时
        // 开启boss战，只会执行一次
        if (boss.openBattle && isSleep)
        {
            stateMachine.ChangeState(boss.rageState);
            isSleep = false;
        }
    }
}
