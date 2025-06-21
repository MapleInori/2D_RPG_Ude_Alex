using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1_PierceState : Boss_1State
{
    public Boss_1_PierceState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_1 _boss) : base(_enemyBase, _stateMachine, _animBoolName, _boss)
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
    }
}
