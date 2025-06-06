using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

/// <summary>
/// ���״̬����״̬�Ļ��࣬����״̬��������Ŀ����߼�
/// </summary>
public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;   // ����Player�ĸ�����������ⷴ��дplayer.rb
    protected float xInput;
    protected float yInput;
    protected float stateTimer; // ��ʱ��
    protected bool triggerCalled;  // �����¼�������
    protected bool dashPreInput; // ���Ԥ����
    private string animBoolName;


    public PlayerState (Player _player, PlayerStateMachine stateMachine,string _animBoolName)
    {
        this.stateMachine = stateMachine;
        this.player = _player;
        this.animBoolName = _animBoolName;
    }

    /// <summary>
    /// ����״̬ʱ����
    /// </summary>
    public virtual void Enter()
    {
        //Debug.Log("Goto " + this.GetType().Name);
        // ����״̬ʱ���ö�������
        player.anim.SetBool(animBoolName,true);
        rb = player.rb;
        triggerCalled = false;
    }
    /// <summary>
    /// ��ǰ״ִ̬��ʲô
    /// </summary>
    public virtual void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity",rb.velocity.y);
        // ״̬֮����Ԥ���봦���Ż��ָ�
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashPreInput = true;
            player.dashDir = Input.GetAxisRaw("Horizontal");
            if (player.dashDir == 0)
                player.dashDir = player.facingDir;
        }

        if(stateTimer >=0)
        {
            stateTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.U) && HasNoSword())
        {
            stateMachine.ChangeState(player.aimSwordState);
            return;
        }
    }

    /// <summary>
    /// �˳�״̬ʱ��ʲô
    /// </summary>
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
        // ��ǰ״̬�˳�ʱ�����ó��Ԥ����
        dashPreInput = false;
    }

    public virtual void AnimationFinishTrigger()
    {
        // �����¼�������
        triggerCalled = true;
    }

    /// <summary>
    /// �������Ƿ��н����еĻ����գ�û�еĻ�������׼
    /// </summary>
    /// <returns></returns>
    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
