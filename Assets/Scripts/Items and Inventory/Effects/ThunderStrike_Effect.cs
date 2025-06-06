using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ThunderStrikeEffect", menuName = "Data/Item Effect/Thunder Strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyTrans)
    {
        // 生产效果
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab,_enemyTrans.position ,Quaternion.identity);

        // 不用在动画播放完后立刻删除这么准确，播放完之后就看不见了，1s后删除就好，反而节省了在动画上加事件，在对象上加脚本
        Destroy(newThunderStrike,1f);
    
    }
}
