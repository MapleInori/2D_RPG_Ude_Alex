using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 克隆技能控制器
/// </summary>
public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    private float cloneFadeTime;
    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = 0.8f;
    private Transform closestEnemy;
    private bool canDuplicateClone;
    private int facingDir = 1;
    private float chanceToDuplicate;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 克隆技能持续时
        if (cloneTimer > 0)
        {
            cloneTimer -= Time.deltaTime;
        }

        // 状态结束，开始消失
        if (cloneTimer <= 0)
        {
            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }

            // 透明度从1到0，持续时间为cloneStayTime，每帧时间间隔为Time.deltaTime。时间过去百分比多少，透明度就减少多少
            sr.color = new Color(1, 1, 1, sr.color.a - Time.deltaTime / cloneFadeTime);

        }
    }

    public void SetupClone
        (Transform _newTransform, float _cloneDuration, float _cloneFadeTime, 
        bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate,float _chanceToDuplicate,Player _player)
    {
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        cloneFadeTime = _cloneFadeTime;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        player = _player;
        // 面向最近的敌人
        FaceClosestEnemy();

        if (_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 4));
        }
    }

    /// <summary>
    /// 动画播放结束时触发调用，如果允许攻击，则攻击后会立刻开始消失
    /// </summary>
    private void AnimationTrigger()
    {
        cloneTimer = 0;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.Instance.clone.CreateClone(hit.transform, new Vector3(0.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestEnemy()
    {
        if (closestEnemy != null)
        {
            // 默认朝右，如果最近的在左边，则转向
            if (closestEnemy.position.x < transform.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
