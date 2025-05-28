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
        // ȷ��Boolֵ�����ã���Ӱ���������״̬
        enemyBase.anim.SetBool("Idle", false);
        enemyBase.anim.SetBool("Move", false);
    }

    public override void Update()
    {
        base.Update();

        // ����ǰ������������ʱ�䣬���ֻ���ڼ��뾶�ڣ�����ʱ�䲻������
        if (enemy.isPlayerDetected())
        {
            stateTimer = enemy.battleTime;
            if (enemy.isPlayerDetected().distance < enemy.attackDistance)
            {
                // attack
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                    // ���أ����������������MoveΪtrue
                    return;
                }
            }
        }
        // ��ʱ��������Ҳ��ڼ�ⷶΧ�������˳�״̬
        if (stateTimer < 0 || (!enemy.isPlayerDetected() && Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.checkRadius))
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;

        // ������ͬһ��X���귴��ת��
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
