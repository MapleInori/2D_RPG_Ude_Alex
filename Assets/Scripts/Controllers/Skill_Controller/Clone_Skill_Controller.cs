using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ��¡���ܿ�����
/// </summary>
public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    private float cloneFadeTime;


    private float cloneTimer;
    private float attackMultiplier;
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
        // ��¡���ܳ���ʱ
        if (cloneTimer > 0)
        {
            cloneTimer -= Time.deltaTime;
        }

        // ״̬��������ʼ��ʧ
        if (cloneTimer <= 0)
        {
            if (sr.color.a <= 0)
            {
                Destroy(gameObject);
            }

            // ͸���ȴ�1��0������ʱ��ΪcloneStayTime��ÿ֡ʱ����ΪTime.deltaTime��ʱ���ȥ�ٷֱȶ��٣�͸���Ⱦͼ��ٶ���
            sr.color = new Color(1, 1, 1, sr.color.a - Time.deltaTime / cloneFadeTime);

        }
    }

    public void SetupClone
        (Transform _newTransform, float _cloneDuration, float _cloneFadeTime, 
        bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate,
        float _chanceToDuplicate,Player _player,float _attackMultiplier)
    {
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;
        cloneFadeTime = _cloneFadeTime;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        player = _player;
        attackMultiplier = _attackMultiplier;
        // ��������ĵ���
        FaceClosestEnemy();

        if (_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 4));
        }
    }

    /// <summary>
    /// �������Ž���ʱ�������ã�������������򹥻�������̿�ʼ��ʧ
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
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                if (player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.Instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }
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
            // Ĭ�ϳ��ң�������������ߣ���ת��
            if (closestEnemy.position.x < transform.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
