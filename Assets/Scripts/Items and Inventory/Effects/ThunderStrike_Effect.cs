using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ThunderStrikeEffect", menuName = "Data/Item Effect/Thunder Strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;

    public override void ExecuteEffect(Transform _enemyTrans)
    {
        // ����Ч��
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab,_enemyTrans.position ,Quaternion.identity);

        // �����ڶ��������������ɾ����ô׼ȷ��������֮��Ϳ������ˣ�1s��ɾ���ͺã�������ʡ���ڶ����ϼ��¼����ڶ����ϼӽű�
        Destroy(newThunderStrike,1f);
    
    }
}
