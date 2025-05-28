using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStike_Controller : MonoBehaviour
{
    private CharacterStats targetStats;
    [SerializeField] private float speed;
    private Animator anim;
    private bool triggered;

    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage,CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    // Update is called once per frame
    void Update()
    {
        // 攻击目标之前，目标死亡
        if(!targetStats)
        {
            DamageAndSelfDestroy();
            return;
        }

        //Time.timeScale = 0.1f;
        if (triggered) return;
        transform.position = Vector2.MoveTowards(transform.position,targetStats.transform.position,speed * Time.deltaTime);
        transform.up =   transform.position - targetStats.transform.position ;
        //Debug.DrawRay(transform.position, transform.right , Color.red); // 应该指向目标
        //Debug.DrawRay(transform.position, targetStat.transform.position - transform.position, Color.green); // 实际方向
        //// TODO:tranform.right的旋转原理
        //Debug.Log(transform.right);

        if(Vector2.Distance(transform.position,targetStats.transform.position) <0.1f)
        {
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector2(3, 3);
            triggered = true;
            anim.SetTrigger("Hit");
            Invoke("DamageAndSelfDestroy",0.2f);
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats?.TakeDamage(damage);
        Destroy(gameObject, 1f);
    }

    /// <summary>TODO:笔记《如果要让物体某个方向朝向目标：2D》————尝试多种方式
    /// 通过观察得知，tranform.right是局部坐标系，当物体旋转时，right也会旋转，所以不适合旋转物体为水平再用right朝向目标，因为right会跟随旋转调整
    /// 应该利用图像是水平还是竖直图像选择使用right还是up.所以这里选择up，并因为up向上，而下方应该朝向目标，所以计算方向取反.
    /// 与之类似的，扔剑技能，扔剑旋转的是子对象，将子对象旋转为水平，然后让父对象右边朝向目标，也可以实现，所以这里也可以修改子对象的Animator为水平
    /// 然后计算并修改父对象right，这也是这里出现这个问题而扔剑没出问题的原因，如果修改子对象，由于后续还需要触发闪电动画，需要重新将animator方向修改回来
    /// 而这里因为修改的父对象的旋转，所以将父对象旋转修改回来即可，如果修改的是子对象的旋转，后续恢复的也应该是子对象的旋转
    /// </summary>
    void OnDrawGizmos()
    {
        // 绘制right轴（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 2);

        // 可选：绘制forward轴（蓝色）和up轴（绿色）
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 1.5f);
    }
}
