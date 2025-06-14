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
        // TODO�����¼��س���ʱ��player�ᶪʧ������Update����player.transform�ᱨ����ʱ��PlayerManager��ͨ��Update���ϼ��player�������������
        // player = PlayerManager.Instance.player.transform;
        // DONE���������Ե�֪����������ʹ����DontDestroyOnLoad(gameObject)���µģ�����������������ӵ�DontDestroyOnLoad(gameObject)��ȥ������û������
        // ����ʹ��DontDestroyOnLoad(gameObject)��ʲô����¿���ʹ���أ�
        // �����޸ĺ��������SkillManager��PlayerManager���Ա���DontDestroyOnLoad(gameObject)��Ȼ��ȥ��������֮����Ե����﷢��û������
        // ���������ط����ܶ�ʧ�����⣬����Ҳ��û���⡣
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
