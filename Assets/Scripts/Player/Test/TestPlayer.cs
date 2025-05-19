using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : TestEntity
{
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;    // �ƶ��ٶ�
    [SerializeField] private float jumpForce;   // ��Ծ����


    private float xInput; // ˮƽ����

    [Header("Attack Info")]
    [SerializeField] private int comboCounter; // ����������
    [SerializeField] private float comboTimeWindow; // ����ʱ�䴰��
    [SerializeField] private float comboWindowTimer; // ����ʱ�䴰�ڼ�ʱ��
    [SerializeField] private bool isAttacking; // �Ƿ��ڹ���

    [Header("DropAttack Info")]
    [SerializeField] private float dropAttackSpeed; // ���乥���ٶ�
    [SerializeField] private int dropAttackPhase; // ���乥���׶�
    [SerializeField] private bool isDropAttacking; // �Ƿ������乥��

    public bool isRun;                          // �Ƿ�����
    [Header("Dash Info")]
    [SerializeField] private float dashSpeed;        // ����ٶ�
    [SerializeField] private float dashDuration;    // ��̳���ʱ��
    [SerializeField] private float dashCooldown;     // �����ȴʱ��
    [SerializeField] private bool isDashing;        // �Ƿ��ڳ��
    private float dashTimer;        // ��̼�ʱ��
    private float dashCooldownTimer; // �����ȴ��ʱ��

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

        // ���
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
        comboWindowTimer = comboTimeWindow; // ��������ʱ�䴰�ڼ�ʱ��
    }

    public void AttackEnd()
    {
        isAttacking = false;
        // �����������ӣ�������һ�ι���
        comboCounter++;
        if (comboCounter > 2)
            comboCounter = 0;
    }

    // ��ʼ���乥��
    private void DropAttack()
    {
        isDropAttacking = true;
        dropAttackPhase = 0;
    }
    // ���乥���׶θ��£�������״̬
    public void DropAttackPhaseUpdate()
    {
        if(dropAttackPhase==0)
        {
            dropAttackPhase = 1;
        }
    }
    // ���乥������
    public void DropAttackEnd()
    {
        isDropAttacking = false;
    }

    private void Move()
    {
        // �ڵ��湥��ʱ�������ƶ�
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
            dashTimer = dashDuration; // ��ʼ���
            isDashing = true;
            dashCooldownTimer = dashCooldown;   // �����ȴ
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
        isRun = Mathf.Abs(rb.velocity.x) > 0.1f; // �ж��Ƿ�����

        anim.SetFloat("yVelocity",rb.velocity.y);
        anim.SetBool("isRun", isRun); 
        anim.SetBool("isGrounded",isGrounded);
        anim.SetBool("isDashing",isDashing); 
        anim.SetBool("isAttacking",isAttacking); // ��������
        anim.SetInteger("comboCounter", comboCounter);
        anim.SetBool("isDropAttacking", isDropAttacking); // ���乥������
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
            comboCounter = 0; // ��������������
        }
    }




}
