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

    // 第一阶段第一帧立刻调用，防止重复播放第一阶段动画，执行该代码后允许第二阶段，第一阶段动画播放完会保留第二阶段的下劈状态
    private void DropAttackAnimUpdate()
    {
        player.DropAttackPhaseUpdate();
    }
    // 落地后切换到攻击动画第三阶段，结束动作，重置状态
    private void DropAttackAnimEnd()
    {
        player.DropAttackEnd();
    }

}
