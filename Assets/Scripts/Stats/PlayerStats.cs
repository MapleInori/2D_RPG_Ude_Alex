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
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;

        if (_damage > GetMaxHealthValue() * .3f)
        {
            //player.SetupKnockbackPower(new Vector2(10, 6));
            //player.fx.ScreenShake(player.fx.shakeHighDamage);


            //int randomSound = Random.Range(34, 35);
            //AudioManager.instance.PlaySFX(randomSound, null);

        }

        ItemData_Equipment currentArmor = Inventory.Instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }
}
