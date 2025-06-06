using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;

    [Header("Leval Details")]
    [SerializeField] private int leval = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = 0.2f;

    protected override void Start()
    {
        ApplyLevalModifiers();
        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();

    }

    private void ApplyLevalModifiers()
    {
        Modity(strength);
        Modity(agility); 
        Modity(intelligence);
        Modity(vitality); 

        Modity(damage);
        //Modity(critChance); 
        //Modity(critPower);

        Modity(maxHealth);
        Modity(armor);
        //Modity(evasion); 
        Modity(magicResistance);

        Modity(fireDamage); 
        Modity(iceDamage);
        Modity(lightingDamage); 
    }

    private void Modity(Stat _stat)
    {
        for (int i = 1; i < leval; i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;

            _stat.AddModifier(Mathf.CeilToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();
        myDropSystem.GenerateDrop(); 
    }
}
