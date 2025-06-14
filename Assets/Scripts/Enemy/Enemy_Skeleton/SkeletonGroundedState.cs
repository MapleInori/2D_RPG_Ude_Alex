using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Enemy_Skeleton enemy;
    private Transform player;
    public SkeletonGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        // TODO：重新加载场景时，player会丢失，导致Update中用player.transform会报错，暂时在PlayerManager中通过Update不断检查player来避免这个问题
        // player = PlayerManager.Instance.player.transform;
        // DONE：经过测试得知，是由于我使用了DontDestroyOnLoad(gameObject)导致的，如果把我自作聪明加的DontDestroyOnLoad(gameObject)都去掉，就没问题了
        // 谨慎使用DontDestroyOnLoad(gameObject)，什么情况下可以使用呢？
        // 在我修改后，至少最后SkillManager和PlayerManager可以保留DontDestroyOnLoad(gameObject)，然后去掉这两个之后测试到这里发现没问题了
        // 至于其他地方可能丢失的问题，估计也会没问题。
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (enemy.isPlayerDetected() || Vector2.Distance(PlayerManager.Instance.player.transform.position,enemy.transform.position) < enemy.checkRadius)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
