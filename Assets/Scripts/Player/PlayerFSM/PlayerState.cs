using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

/// <summary>
/// 玩家状态机中状态的基类，各种状态管理自身的控制逻辑
/// </summary>
public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;   // 引用Player的刚体组件，避免反复写player.rb
    protected float xInput;
    protected float yInput;
    protected float stateTimer; // 计时器
    protected bool triggerCalled;  // 动画事件触发器
    protected bool dashPreInput; // 冲刺预输入
    private string animBoolName;


    public PlayerState (Player _player, PlayerStateMachine stateMachine,string _animBoolName)
    {
        this.stateMachine = stateMachine;
        this.player = _player;
        this.animBoolName = _animBoolName;
    }

    /// <summary>
    /// 进入状态时调用
    /// </summary>
    public virtual void Enter()
    {
        //Debug.Log("Goto " + this.GetType().Name);
        // 进入状态时设置动画参数
        player.anim.SetBool(animBoolName,true);
        rb = player.rb;
        triggerCalled = false;
    }
    /// <summary>
    /// 当前状态执行什么
    /// </summary>
    public virtual void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        player.anim.SetFloat("yVelocity",rb.velocity.y);
        // 状态之间冲刺预输入处理，优化手感
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
    /// 退出状态时做什么
    /// </summary>
    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
        // 当前状态退出时，重置冲刺预输入
        dashPreInput = false;
    }

    public virtual void AnimationFinishTrigger()
    {
        // 动画事件触发器
        triggerCalled = true;
    }

    /// <summary>
    /// 场景中是否有剑，有的话回收，没有的话才能瞄准
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
