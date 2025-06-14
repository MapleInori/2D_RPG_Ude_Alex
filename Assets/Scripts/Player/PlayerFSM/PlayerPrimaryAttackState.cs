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
        // Time.timeȫ�ֱ���������ֻ��Ҫ������󹥻�ʱ��͹��������ڼ����ж�������������Ҫ��ʱ��
        if (comboCounter > 2 || Time.time > lastAttackTime + attackWindow)
        {
            comboCounter = 0;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        // �л�����������Ϊ�����ٶȷ������������Զ����Ʒ�ת��������ʱ���й��������ϵ��ٶȣ��������ﲻ��Ҫ���⴦��ת
        float attackDir = player.facingDir;
        if(xInput != 0)
        {
            attackDir = xInput;
        }

        
        // �������ƶ�ƫ��
        player.SetVelocity(player.attackMovement[comboCounter] * attackDir, rb.velocity.y);
        // �����ٶȿ����޸ģ����Ӷ����ٶȼ��ɣ��˳�ʱ����
        player.anim.speed = player.baseAttackSpeed + player.extraAttackSpeed;
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
        // ���0.15f�Ĺ�����ҡ�������ƶ�������ʵ������dash��ϣ����ʱ��������ʱ��
        player.StartCoroutine("BusyFor", 0.15f);
        comboCounter++;
        lastAttackTime = Time.time;
        // �����ٶ�����
        player.anim.speed = player.baseAttackSpeed;
    }

    public override void Update()
    {
        base.Update();

        // ԭ���ƶ�������£�0.1f��ͣ����
        if(stateTimer <0)
        {
            player.SetZeroVelocity();
        }
        // ��������ʱ�л���idle����idle�ٻص��ڶ��ι���
        if(triggerCalled)
        {
            // Ԥ�������л����
            if (dashPreInput && player.dashUsageTimer == 0f)
            {
                stateMachine.ChangeState(player.dashState);
                return;
            }
            stateMachine.ChangeState(player.idleState);
        }
    }
}
