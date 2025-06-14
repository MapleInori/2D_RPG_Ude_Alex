using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationFinishTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.Instance.PlaySFX(2,null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                if (_target != null)
                {
                    player.stats.DoDamage(_target);

                }


                Inventory.Instance.GetEquipment(EquipmentType.Weapon)?.Effect(_target.transform);

            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.Instance.sword.CreateSword();
    }
}
