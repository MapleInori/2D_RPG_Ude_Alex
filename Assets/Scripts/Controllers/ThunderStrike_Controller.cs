using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrike_Controller : MonoBehaviour
{

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.Instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            playerStats.DoMagicalDamage(enemyStats);
        }
    }
}
