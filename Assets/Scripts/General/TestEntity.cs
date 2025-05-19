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
    [SerializeField] protected Transform groundCheck; // 地面检测点
    [SerializeField] protected float groundCheckDistance; // 地面检测距离
    [SerializeField] protected Transform wallCheck; // 墙壁检测点
    [SerializeField] protected float wallCheckDistance; // 墙壁检测距离
    [SerializeField] protected LayerMask groundLayer; // 地面层
    [SerializeField] protected bool isGrounded; // 是否在地面上
    [SerializeField] protected bool isWallDected; // 是否检测到墙壁

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
        transform.Rotate(0f, 180f, 0f); // 翻转角色
    }

    protected virtual void CollisionCheck()
    {
        // 检测是否在地面上
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
        isWallDected = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir,wallCheckDistance, groundLayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance * facingDir);
    }
}
