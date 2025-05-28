using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private CharacterStats characterStats;
    private RectTransform rectTransform;
    private Slider slider;

    // �������ȡ������߸����ȡ�����������Awake��������Ӽ���ȡ�������Awake�ᱨ���Ӽ�����δʵ����
    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        characterStats = GetComponentInParent<CharacterStats>();
        rectTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

    }
    /// <summary>
    /// ����������CharacterStat��ִ�У����¿�Ѫ״̬������ͨ��Edit-Project Settings -> Script Execution Order ������ִ��˳��
    /// </summary>
    private void Start()
    {
        UpdateHealthUI();
    }

    private void OnEnable()
    {
        entity.onFliped += FlipUI;
        characterStats.onHealthChanged += UpdateHealthUI;
    }



    private void UpdateHealthUI()
    {
        slider.maxValue =characterStats.GetMaxHealthValue();
        slider.value = characterStats.currentHealth;
    }

    private void FlipUI()
    {
        //Debug.Log("UI Flip");
        rectTransform.Rotate(0, 180, 0);
    }

    private void OnDisable()
    {
        entity.onFliped -= FlipUI;
        characterStats.onHealthChanged -= UpdateHealthUI;
    }


    /// TODO:�������Ը�Debuff��Ӷ�Ӧ��ͼ����Ѫ����
}
