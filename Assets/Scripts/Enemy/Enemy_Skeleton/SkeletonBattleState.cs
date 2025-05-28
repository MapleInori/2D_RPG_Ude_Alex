using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Enemy_Skeleton enemy;
    private int moveDir;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.Instance.player.transform;
        stateTimer = enemy.battleTime;
    }

    public override void Exit()
    {
        base.Exit();
        // 确保Bool值被重置，不影响进入其他状态
        enemyBase.anim.SetBool("Idle", false);
        enemyBase.anim.SetBool("Move", false);
    }

    public override void Update()
    {
        base.Update();

        // 在正前方会重置索敌时间，如果只是在检测半径内，索敌时间不会重置
        if (enemy.isPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.isPlayerDetected().distance < enemy.attackDistance)
            {
                // attack
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                    // 返回，避免后续重新设置Move为true
                    return;
                }
            }
        }
        // 超时，或者玩家不在检测范围内立刻退出状态
        if (stateTimer < 0 || (!enemy.isPlayerDetected() && Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.checkRadius))
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        // 避免在同一个X坐标反复转身
        if (Mathf.Abs(enemy.transform.position.x - player.transform.position.x) > 0.1f)
        {
            enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
            enemyBase.anim.SetBool(animBoolName, true);
            enemyBase.anim.SetBool("Idle", false);
        }
        else
        {
            enemyBase.anim.SetBool(animBoolName, false);
            enemyBase.anim.SetBool("Idle", true);
        }
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }
}
