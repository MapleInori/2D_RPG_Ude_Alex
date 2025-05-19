using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEntity : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;

    protected float facingDir = 1;
    protected bool facingRight = true;

    [Header("Collision Info")]
    [SerializeField] protected Transform groundCheck; // �������
    [SerializeField] protected float groundCheckDistance; // ���������
    [SerializeField] protected Transform wallCheck; // ǽ�ڼ���
    [SerializeField] protected float wallCheckDistance; // ǽ�ڼ�����
    [SerializeField] protected LayerMask groundLayer; // �����
    [SerializeField] protected bool isGrounded; // �Ƿ��ڵ�����
    [SerializeField] protected bool isWallDected; // �Ƿ��⵽ǽ��

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CollisionCheck();
    }

    protected virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f); // ��ת��ɫ
    }

    protected virtual void CollisionCheck()
    {
        // ����Ƿ��ڵ�����
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
        isWallDected = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir,wallCheckDistance, groundLayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance * facingDir);
    }
}
