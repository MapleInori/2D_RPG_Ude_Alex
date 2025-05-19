using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    private TestPlayer player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<TestPlayer>();
    }

    private void AttackAnimEnd()
    {
        player.AttackEnd();
    }

    // ��һ�׶ε�һ֡���̵��ã���ֹ�ظ����ŵ�һ�׶ζ�����ִ�иô��������ڶ��׶Σ���һ�׶ζ���������ᱣ���ڶ��׶ε�����״̬
    private void DropAttackAnimUpdate()
    {
        player.DropAttackPhaseUpdate();
    }
    // ��غ��л����������������׶Σ���������������״̬
    private void DropAttackAnimEnd()
    {
        player.DropAttackEnd();
    }

}
