using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public enum StatType
{
    Strength,
    Agility,
    Intelligence,
    Vitality,
    Damage,
    CritChance,
    CritPower,
    Health,
    Armor,
    Evasion,
    MagicResistance,
    FireDamage,
    IceDamage,
    LightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;   // 1��������1�㹥����0.1%�����˺�
    public Stat agility;    // 1�����ݼ�0.1%���ܺͱ�����
    public Stat intelligence;   // 1��������1�����˺���1ħ������
    public Stat vitality;   // 1������������3������ֵ

    // TODO:�������ת�������ֶΣ�����ʹ�ø��ֶμ��㣬�����޸�

    [Header("Offensive Stats")]
    public Stat damage;
    public Stat critChance; // �����ʣ��ٷֱ�
    public Stat critPower;  // �����˺����ٷֱȣ�������ֵ150%

    [Header("Defensive stats")]
    public Stat maxHealth;  // ����������֮ǰ���������ֵ
    public Stat armor;  // ����
    public Stat evasion;    // ���ܼ��ʣ��ٷֱ�
    public Stat magicResistance; // ħ������

    [Header("Magic Stats")]
    public Stat fireDamage; // �����˺�
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;  // �������˵�Ѫ
    public bool isChilled;  // ���׽���20%
    public bool isShocked;  // �����ʽ���20%


    private float ignitedTimer; // ����״̬��ʱ��
    private float chilledTimer; // ����״̬��ʱ��
    private float shockedTimer; // 

    public float ignitedDuration = 4f;
    public float chilledDuration = 4f;
    public float shockedDuration = 4f;

    private float igniteDamageCoolDown = 0.3f;  // �����˺�ʱ����
    private float igniteDamageTimer; // �����˺���ʱ��
    private int igniteDamage; // ÿ�������˺���Ϊ�����˺���20%

    [SerializeField] private GameObject shockStrikePrefab;
    private int shockeDamage;

    public float chillSlowPercentage = 0.2f;

    public int currentHealth;

    public UnityAction onHealthChanged;

    public bool isDead { get; private set; }



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

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        // start corototuine for stat increase
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    /// <summary>
    /// ��Ŀ������˺������㻤��
    /// </summary>
    /// <param name="_targetStats">Ŀ���״̬����</param>
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;
        string attackLog = "��ʼ�˺���";
        float totalDamage = damage.GetValue() + strength.GetValue();
        attackLog += totalDamage;
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            attackLog += "������\n";
        }


        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        // �����˺�ʱ����������Ϊ����
        _targetStats.TakeDamage(Mathf.RoundToInt( totalDamage));
        attackLog += "ʵ���˺���" + totalDamage;
        Debug.Log(attackLog);
        // ���������ħ�����ԣ���������ħ���˺���Ŀǰ��û�У���Ҫ��ˮ�����ħ���˺�
        DoMagicDamage(_targetStats);// ע������չ��Ƿ����ħ���˺�
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
    /// TODO:Ailment����Ҳ���޸�ΪDebuff
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
                // �����ǰ��������ң�����Ч����Ȼ��������������˺���������ң����������繥������
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

        // �ҵ�����ĵ��ˣ�����ų��Լ���
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
            if (closestEnemy == null)    // ���û���������ˣ���Ŀ��Ϊ��ǰ����
            {
                closestEnemy = transform;
            }
        }


        if (closestEnemy != null)
        {
            // ���˵��������shockStrikePrefab�������Լ�
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            // TODO:�ʼǡ���ȡ���࣬����ֻ�����࣬Ϊʲô���ԣ�ת���߼���ʲô����
            newShockStrike.GetComponent<ShockStike_Controller>().Setup(shockeDamage, closestEnemy.GetComponent<CharacterStats>());
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
    /// ���������˺�ֵ������Ҳ�������ó���ʱ����˺����
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
    /// �ܵ��˺�����DoDamage���ã��˺�ֵΪ�����˺�ֵ���������ʵ�˺���ֱ�ӵ�������Ҳ�аգ�
    /// </summary>
    /// <param name="_damage">�ܵ��˺���ֵ</param>
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
    //TODO:�����˺����ʵ���أ�
    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        //Debug.Log("currentHeal:" + currentHealth);
        onHealthChanged?.Invoke();

    }

    public virtual void IncreaseHealthBy(int _health)
    {
        // TODO:�����������Ч�����棬�����ɫӵ������Ч�����Ӱٷ�֮���٣���������⴦��
        currentHealth += _health;
        if(currentHealth > GetMaxHealthValue())
        {
            currentHealth = GetMaxHealthValue();
        }
        // ����Ѫ��UI
        onHealthChanged?.Invoke();

    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region Stat Calculations

    private float CheckTargetArmor(CharacterStats _targetStats, float totalDamage)
    {
        if (_targetStats.isChilled)
        {
            totalDamage -= _targetStats.armor.GetValue() * 0.8f; // Ŀ�괦�ڱ���״̬�����׽���20%
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();

        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue); // ȷ���˺���Ϊ����
        return totalDamage;
    }

    private int CheckTargetResistant(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 1);

        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        float totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue() * 0.001f;

        if (isShocked)
        {
            totalEvasion += 20; // ���ڵ��״̬�������ʽ���20%���൱�ڶԷ����20%���ܼ���
        }

        if (Random.Range(0, 100) < totalEvasion)
        {

            return true;
        }
        return false;
    }

    private bool CanCrit()
    {
        float totalCritChance = critChance.GetValue() + agility.GetValue() * 0.001f;
        if (Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }

        return false;
    }

    private float CalculateCriticalDamage(float _damage)
    {
        float totalCritPower = critPower.GetValue()*0.01f + strength.GetValue() * 0.001f;

        float criticalDamage = _damage * totalCritPower;

        return criticalDamage;

    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 3;
    }

    #endregion

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.Strength) return strength;
        else if (_statType == StatType.Agility) return agility;
        else if (_statType == StatType.Intelligence) return intelligence;
        else if (_statType == StatType.Vitality) return vitality;
        else if (_statType == StatType.Damage) return damage;
        else if (_statType == StatType.CritChance) return critChance;
        else if (_statType == StatType.CritPower) return critPower;
        else if (_statType == StatType.Health) return maxHealth;
        else if (_statType == StatType.Armor) return armor;
        else if (_statType == StatType.Evasion) return evasion;
        else if (_statType == StatType.MagicResistance) return magicResistance;
        else if (_statType == StatType.FireDamage) return fireDamage;
        else if (_statType == StatType.IceDamage) return iceDamage;
        else if (_statType == StatType.LightingDamage) return lightingDamage;

        return null;
    }
}
