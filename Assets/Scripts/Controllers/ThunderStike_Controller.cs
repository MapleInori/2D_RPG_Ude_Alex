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
        // ����Ŀ��֮ǰ��Ŀ������
        if(!targetStats)
        {
            DamageAndSelfDestroy();
            return;
        }

        //Time.timeScale = 0.1f;
        if (triggered) return;
        transform.position = Vector2.MoveTowards(transform.position,targetStats.transform.position,speed * Time.deltaTime);
        transform.up =   transform.position - targetStats.transform.position ;
        //Debug.DrawRay(transform.position, transform.right , Color.red); // Ӧ��ָ��Ŀ��
        //Debug.DrawRay(transform.position, targetStat.transform.position - transform.position, Color.green); // ʵ�ʷ���
        //// TODO:tranform.right����תԭ��
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

    /// <summary>TODO:�ʼǡ����Ҫ������ĳ��������Ŀ�꣺2D�������������Զ��ַ�ʽ
    /// ͨ���۲��֪��tranform.right�Ǿֲ�����ϵ����������תʱ��rightҲ����ת�����Բ��ʺ���ת����Ϊˮƽ����right����Ŀ�꣬��Ϊright�������ת����
    /// Ӧ������ͼ����ˮƽ������ֱͼ��ѡ��ʹ��right����up.��������ѡ��up������Ϊup���ϣ����·�Ӧ�ó���Ŀ�꣬���Լ��㷽��ȡ��.
    /// ��֮���Ƶģ��ӽ����ܣ��ӽ���ת�����Ӷ��󣬽��Ӷ�����תΪˮƽ��Ȼ���ø������ұ߳���Ŀ�꣬Ҳ����ʵ�֣���������Ҳ�����޸��Ӷ����AnimatorΪˮƽ
    /// Ȼ����㲢�޸ĸ�����right����Ҳ������������������ӽ�û�������ԭ������޸��Ӷ������ں�������Ҫ�������綯������Ҫ���½�animator�����޸Ļ���
    /// ��������Ϊ�޸ĵĸ��������ת�����Խ���������ת�޸Ļ������ɣ�����޸ĵ����Ӷ������ת�������ָ���ҲӦ�����Ӷ������ת
    /// </summary>
    void OnDrawGizmos()
    {
        // ����right�ᣨ��ɫ��
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 2);

        // ��ѡ������forward�ᣨ��ɫ����up�ᣨ��ɫ��
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 1.5f);
    }
}
