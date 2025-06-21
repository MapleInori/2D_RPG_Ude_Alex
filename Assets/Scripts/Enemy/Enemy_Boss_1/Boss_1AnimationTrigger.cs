using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1AnimationTrigger :MonoBehaviour
{
    private Boss_1 boss => GetComponentInParent<Boss_1>();

    private void AnimationTrigger()
    {
        boss.AnimationFinishTrigger();
    }
}
