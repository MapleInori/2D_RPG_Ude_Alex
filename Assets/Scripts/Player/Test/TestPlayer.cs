using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : TestEntity
{
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;    // 移动速度
    [SerializeField] private float jumpForce;   // 跳跃力度


    private float xInput; // 水平输入

    [Header("Attack Info")]
    [SerializeField] private int comboCounter; // 连击计数器
    [SerializeField] private float comboTimeWindow; // 连击时间窗口
    [SerializeField] private float comboWindowTimer; // 连击时间窗口计时器
    [SerializeField] private bool isAttacking; // 是否在攻击

    [Header("DropAttack Info")]
    [SerializeField] private float dropAttackSpeed; // 下落攻击速度
    [SerializeField] private int dropAttackPhase; // 下落攻击阶段
    [SerializeField] private bool isDropAttacking; // 是否在下落攻击

    public bool isRun;                          // 是否在跑
    [Header("Dash Info")]
    [SerializeField] private float dashSpeed;        // 冲刺速度
    [SerializeField] private float dashDuration;    // 冲刺持续时间
    [SerializeField] private float dashCooldown;     // 冲刺冷却时间
    [SerializeField] private bool isDashing;        // 是否在冲刺
    private float dashTimer;        // 冲刺计时器
    private float dashCooldownTimer; // 冲刺冷却计时器

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Move();
        CheckInput();
        FlipController();
        PlayAnimation();
        TimerUpdate();
    }

    private void FixedUpdate()
    {

    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if(isDropAttacking && dropAttackPhase ==0)
        {
            rb.velocity = new Vector2(0, 0);
        }
        else if(isDropAttacking && dropAttackPhase == 1)
        {
            rb.velocity = new Vector2(0, -dropAttackSpeed);
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            if(isGrounded && !isDropAttacking)
            {
                Attack();
            }
            else if(!isGrounded && !isDropAttacking)
            {
                DropAttack();
            }
        }

        // 冲刺
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashAbility();
        }

        // Jump
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

    }

    private void Attack()
    {
        isAttacking = true;
        comboWindowTimer = comboTimeWindow; // 重置连击时间窗口计时器
    }

    public void AttackEnd()
    {
        isAttacking = false;
        // 连击计数增加，允许下一段攻击
        comboCounter++;
        if (comboCounter > 2)
            comboCounter = 0;
    }

    // 开始下落攻击
    private void DropAttack()
    {
        isDropAttacking = true;
        dropAttackPhase = 0;
    }
    // 下落攻击阶段更新，到保持状态
    public void DropAttackPhaseUpdate()
    {
        if(dropAttackPhase==0)
        {
            dropAttackPhase = 1;
        }
    }
    // 下落攻击结束
    public void DropAttackEnd()
    {
        isDropAttacking = false;
    }

    private void Move()
    {
        // 在地面攻击时，不许移动
        if(isAttacking && isGrounded)
        {
            rb.velocity = new Vector2(0,0);
        }
        // Dash
        else if( dashTimer > 0 )
        {
            rb.velocity = new Vector2(facingDir * dashSpeed, 0);
        }
        else// Run
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
        }

    }

    private void DashAbility()
    {
        if(dashCooldownTimer <= 0 && !isAttacking && !isDropAttacking)
        {
            dashTimer = dashDuration; // 开始冲刺
            isDashing = true;
            dashCooldownTimer = dashCooldown;   // 冲刺冷却
        }
    }

    private void Jump()
    {
        if (isGrounded && !isAttacking && !isDropAttacking)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void PlayAnimation()
    {
        isRun = Mathf.Abs(rb.velocity.x) > 0.1f; // 判断是否在跑

        anim.SetFloat("yVelocity",rb.velocity.y);
        anim.SetBool("isRun", isRun); 
        anim.SetBool("isGrounded",isGrounded);
        anim.SetBool("isDashing",isDashing); 
        anim.SetBool("isAttacking",isAttacking); // 攻击动画
        anim.SetInteger("comboCounter", comboCounter);
        anim.SetBool("isDropAttacking", isDropAttacking); // 下落攻击动画
        anim.SetInteger("dropAttackPhase", dropAttackPhase);
    }

    

    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }




    private void TimerUpdate()
    {
        dashCooldownTimer -= Time.deltaTime;
        if(dashCooldownTimer <0)
        {
            dashCooldownTimer = 0;
        }

        dashTimer -= Time.deltaTime;
        if(dashTimer<0)
        {
            dashTimer = 0;
            isDashing = false;
        }

        comboWindowTimer -= Time.deltaTime;
        if(comboWindowTimer <0)
        {
            comboWindowTimer = 0;
            comboCounter = 0; // 重置连击计数器
        }
    }




}
