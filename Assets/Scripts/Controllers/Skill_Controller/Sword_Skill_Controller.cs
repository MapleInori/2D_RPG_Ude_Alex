using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private float delayDestroyTime;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D coll;
    private Player player;

    private bool canRotate;
    private bool isReturning;

    private float returnSpeed;
    private float freezeTimeDuration;

    [Header("Pierce Info")]
    [SerializeField] private int pierceAmount;


    [Header("Bounce Info")]
    private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    public List<Transform> enemyTargets;
    private int targetIndex;

    [Header("Spin Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;


    private float hitTimer;
    private float hitCoolDown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        // ���ⱳ��ǽʱ�Ӳ���ȥ
        coll.radius = 0.1f;
        StartCoroutine(ChangeColliderRadius());
        canRotate = true;
    }

    private IEnumerator ChangeColliderRadius()
    {
        yield return new WaitForSeconds(0.1f); 
        coll.radius = 0.4f;
    }

    private void Start()
    {
        // ������10s
        Invoke("DestroyMe",10f);
    }

    public void SetupSword(Vector2 _dir, float _gravity, Player _player, float _delayDestroyTime,float _freezeTimeDuration,float _returnSpeed)
    {
        player = _player;
        returnSpeed = _returnSpeed;
        freezeTimeDuration = _freezeTimeDuration;
        rb.velocity = _dir;
        rb.gravityScale = _gravity;
        delayDestroyTime = _delayDestroyTime;

        if (pierceAmount <= 0)
        {
            anim.SetBool("Rotation", true);

        }

        spinDirection = Mathf.Clamp(rb.velocity.x,-1,1);
    }

    public void SetupBounce(bool _isBouncing, int _amountOfBounce,float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bounceSpeed = _bounceSpeed;
        enemyTargets = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance,float _spinDuration,float _hitCoolDown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCoolDown = _hitCoolDown;
    }

    public void ReturnSword()
    {
        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void Update()
    {
        if (canRotate)
        {
            // ����ʼָ���ұߣ�ֻҪ���ұߵķ���һֱ�����ٶȵķ��򼴿�
            transform.right = rb.velocity;

        }

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1f)
            {
                player.CatchTheSword();
                isReturning = false;
            }
        }

        BounceLogic();
        SpinningLogic();
    }

    private void SpinningLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpin();
            }
            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x + spinDirection,transform.position.y),1.5f * Time.deltaTime);
                Debug.Log(spinTimer);
                if (spinTimer < 0)
                {
                    spinTimer = 0;
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;
                if (hitTimer <= 0)
                {
                    hitTimer = hitCoolDown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpin()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        // ������OnTriggerEnter2D�ﷴ������ʱ��
        if (spinTimer <=0)
            spinTimer = spinDuration;
    }

    private void DestroyMe()
    {
       Destroy(gameObject);
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTargets.Count != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTargets[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTargets[targetIndex].position) < 0.1f)
            {
                // TODO��Ϊʲô����֮�󲻻ᴥ��OnTriggerEnter2D��������Ҫ������ã�
                SwordSkillDamage(enemyTargets[targetIndex].GetComponent<Enemy>());
                targetIndex++;
                bounceAmount--;
                if (targetIndex >= enemyTargets.Count)
                {
                    targetIndex = 0;
                }
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning) return;

        //TODO:Ϊʲô�����ʱ��û��Ч���������ڵ����߼�����ò��У�����Ϊʲô��������
        //Debug.Log("Attack:"+collision.gameObject.name);
        if(collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        // ���������ˣ���ʼ����Ȼ����
        SetupTargetForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>());
        enemy.StartCoroutine("FreezeTimeFor", freezeTimeDuration);
    }

    private void SetupTargetForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTargets.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTargets.Add(hit.transform);
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }
        if (isSpinning)
        {
            // ���е�һ������ͣ��
            StopWhenSpin();
            return;
        }
        canRotate = false;
        coll.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // ������ڵ��䣬��ֹͣ���������û�����е��ˣ��򲻻����У���������Ϊ0
        if (isBouncing && enemyTargets.Count > 0) return;

        anim.SetBool("Rotation", false);
        transform.SetParent(collision.transform);
    }
}
