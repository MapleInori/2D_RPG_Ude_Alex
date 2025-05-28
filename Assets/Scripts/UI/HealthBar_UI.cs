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

    // 从自身获取组件或者父类获取组件，可以在Awake，如果从子级获取组件，在Awake会报错，子级可能未实例化
    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        characterStats = GetComponentInParent<CharacterStats>();
        rectTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

    }
    /// <summary>
    /// 会出现这里比CharacterStat先执行，导致空血状态，可以通过Edit-Project Settings -> Script Execution Order 来调整执行顺序
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


    /// TODO:后续可以给Debuff添加对应的图标在血条上
}
