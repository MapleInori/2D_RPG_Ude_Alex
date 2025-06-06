using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item Effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;
    // 当第三次攻击命中敌人时才出发，因为在普攻状态里有敌人检测判定，判断攻击到敌人时才会调用后续的伤害和技能效果


    public override void ExecuteEffect(Transform _respawnPosition)
    {
        Player player = PlayerManager.Instance.player;

        bool thirdAttack = player.primaryAttackState.comboCounter == 2;

        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, 10);
        }
    }
}
