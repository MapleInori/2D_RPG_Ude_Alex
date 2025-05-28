using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ܹ�������ͨ�����������ü���
/// </summary>
public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;
    public static SkillManager Instance
    {
        get { return _instance; }
    }

    public Dash_Skill dash { get; private set; }
    public Clone_Skill clone { get; private set; }
    public Sword_Skill sword { get; private set; }
    public BlackHole_Skill blackHole { get; private set; }
    public Crystal_Skill crystal { get; private set; }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // ��������Ƿ�糡������
    }

    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
        sword = GetComponent<Sword_Skill>();
        blackHole = GetComponent<BlackHole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
    }
}
