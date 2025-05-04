using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private float moveSpeed;    // 移动速度
    [SerializeField] private float jumpForce;   // 跳跃力度

    private float facingDir = 1;
    private bool facingRight = true;
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


    [Header("Collision Info")]
    [SerializeField] private LayerMask groundLayer; // 地面层
    [SerializeField] private float groundCheckDistance; // 地面检测距离
    private bool isGrounded; // 是否在地面上

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckInput();
        FlipController();
        CollisionCheck();
        PlayAnimation();
        TimerUpdate();
    }

    private void FixedUpdate()
    {

    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if(isDropAttacking)
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

    private void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f); // 翻转角色
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

    private void CollisionCheck()
    {
        // 检测是否在地面上
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }


}
