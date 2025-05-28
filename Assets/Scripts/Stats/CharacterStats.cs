using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;   // 1点力量加一点攻击和1%暴击伤害
    public Stat agility;    // 1点敏捷加1%闪避和暴击率
    public Stat intelligence;   // 1点智力加1法术伤害和1%魔法抗性
    public Stat vitality;   // 1点耐力增加3点生命值

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance;//暴击率
    public Stat critPower;  // 暴击伤害  基础数值150%

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance; // 魔法抗性

    [Header("Magic Stats")]
    public Stat fireDamage; // 火焰伤害
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;  // 持续烧伤掉血
    public bool isChilled;  // 护甲降低20%
    public bool isShocked;  // 命中率降低20%


    private float ignitedTimer; // 烧伤状态计时器
    private float chilledTimer; // 冰冻状态计时器
    private float shockedTimer; // 

    public float ignitedDuration = 4f;
    public float chilledDuration = 4f;
    public float shockedDuration = 4f;

    private float igniteDamageCoolDown = 0.3f;  // 烧伤伤害时间间隔
    private float igniteDamageTimer; // 烧伤伤害计时器
    private int igniteDamage; // 每段烧伤伤害，为火焰伤害的20%

    [SerializeField] private GameObject shockStrikePrefab;
    private int shockeDamage;

    public float chillSlowPercentage = 0.2f;

    public int currentHealth;

    public UnityAction onHealthChanged;

    protected bool isDead;

    protected virtual void Start()
    {
        critPower.SetDefaltValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        DebuffTimer();
        if (isIgnited)
        {
            ApplyIgniteDamage();

        }
    }


    private void DebuffTimer()
    {
        if (ignitedTimer > 0)
        {
            ignitedTimer -= Time.deltaTime;
        }
        else if (ignitedTimer < 0)
        {
            ignitedTimer = 0;
            isIgnited = false;
        }
        if (chilledTimer > 0)
        {
            chilledTimer -= Time.deltaTime;
        }
        else if (chilledTimer < 0)
        {
            chilledTimer = 0;
            isChilled = false;
        }
        if (shockedTimer > 0)
        {
            shockedTimer -= Time.deltaTime;
        }
        else if (shockedTimer < 0)
        {
            shockedTimer = 0;
            isShocked = false;
        }
    }

    /// <summary>
    /// 对目标造成伤害，计算护甲
    /// </summary>
    /// <param name="_targetStats">目标的状态数据</param>
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }


        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);

        // 如果武器有魔法属性，则可以造成魔法伤害，目前还没有，主要由水晶造成魔法伤害
        //DoMagicDamage(_targetStats);
    }

    #region Magical Damage and Ailments
    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicDamage = CheckTargetResistant(_targetStats, totalMagicDamage);
        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0) return;

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyChill && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                break;
            }
            if (Random.value < 0.33f && _iceDamage > 0)
            {
                canApplyChill = true;
                break;
            }
            if (Random.value < 0.33f && _lightingDamage > 0)
            {
                canApplyShock = true;
                break;
            }
        }


        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));
        }
        if (canApplyShock)
        {
            _targetStats.SetupShockDamage(Mathf.RoundToInt(_lightingDamage * 0.1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }


    /// <summary>
    /// TODO:Ailment后续也许修改为Debuff
    /// </summary>
    /// <param name="_ignite"></param>
    /// <param name="_chill"></param>
    /// <param name="_shock"></param>
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ignitedDuration;
            fx.IgniteFxFor(ignitedDuration);


        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = chilledDuration;

            GetComponent<Entity>().SlowEntityBy(chillSlowPercentage, chilledDuration);
            fx.ChillFxFor(chilledDuration);
        }
        if (_shock && canApplyShock)
        {
            Debug.Log("ApplyAilments");

            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                // 如果当前对象是玩家，则不生效。不然如果敌人有闪电伤害并攻击玩家，则会产生闪电攻击敌人
                if (GetComponent<Player>() != null)
                {
                    return;
                }

                HitNearestTargetWithShockStrike();

            }
        }

    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked) return;
        isShocked = _shock;
        shockedTimer = shockedDuration;
        fx.ShockFxFor(shockedDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 15);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // 找到最近的敌人，如果排除自己？
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 0.001f)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
            if (closestEnemy == null)    // 如果没有其他敌人，则目标为当前敌人
            {
                closestEnemy = transform;
            }
        }


        if (closestEnemy != null)
        {
            // 敌人调用自身的shockStrikePrefab来攻击自己
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            // TODO:笔记《获取父类，但是只有子类，为什么可以？转换逻辑是什么？》
            newShockStrike.GetComponent<ThunderStike_Controller>().Setup(shockeDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer > 0)
        {
            igniteDamageTimer -= Time.deltaTime;
        }
        else if (igniteDamageTimer <= 0)
        {
            DecreaseHealthBy(igniteDamage);
            if (currentHealth < 0 && !isDead)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCoolDown;

        }
    }
    /// <summary>
    /// 设置烧伤伤害值，后续也可以设置持续时间和伤害间隔
    /// </summary>
    /// <param name="_damage"></param>
    public void SetupIgniteDamage(int _damage)
    {
        igniteDamage = _damage;
    }

    public void SetupShockDamage(int _damage)
    {
        shockeDamage = _damage;
    }

    #endregion

    /// <summary>
    /// 受到伤害，由DoDamage调用，伤害值为最终伤害值，如果有真实伤害，直接调用这里也行罢？
    /// </summary>
    /// <param name="_damage">受到伤害的值</param>
    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);
        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        //Debug.Log("Damage:" + _damage);
        if (currentHealth < 0 && !isDead)
        {
            Die();
        }

    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        Debug.Log("currentHeal:" + currentHealth);
        onHealthChanged?.Invoke();

    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region Stat Calculations

    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f); // 目标处于冰冻状态，护甲降低20%
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();

        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue); // 确保伤害不为负数
        return totalDamage;
    }

    private int CheckTargetResistant(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);

        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20; // 处于电击状态，命中率降低20%，相当于对方提高20%闪避几率
        }

        if (Random.Range(0, 100) < totalEvasion)
        {

            return true;
        }
        return false;
    }

    private bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;

        float criticalDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(criticalDamage);

    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 3;
    }

    #endregion
}
