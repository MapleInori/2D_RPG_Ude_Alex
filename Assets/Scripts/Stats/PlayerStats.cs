using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        player.DamageImpact();
    }

    protected override void Die()
    {
        base.Die();

        player.Die();

        GameManager.Instance.lostCurrencyAmount = PlayerManager.Instance.currency;
        PlayerManager.Instance.currency = 0;
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    public override void KillSelf()
    {
        base.KillSelf();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;
        // 玩家收到高额伤害设置被击退和画面震动，然后效果中应用
        if (_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(10, 6));
            player.knockbackDuration = 0.3f;
            //player.fx.ScreenShake(player.fx.shakeHighDamage);


            int randomSound = Random.Range(34, 36);
            Debug.Log(randomSound);
            AudioManager.Instance.PlaySFX(randomSound, null);

        }

        ItemData_Equipment currentArmor = Inventory.Instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        float totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = totalDamage * _multiplier;

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(Mathf.RoundToInt(totalDamage));


        DoMagicalDamage(_targetStats); // remove if you don't want to apply magic hit on primary attack
    }
}
