using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skills;


    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 233;

    // TODO：不在技能图标上放按键文字，有点遮挡，打算放设置里

    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.Instance;
    }

    void Update()
    {
        UpdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.I) && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);
        // 收剑之后进入CD，或者不设置CD
        if (Input.GetKeyDown(KeyCode.U) && skills.sword.swordUnlocked)
            SetCooldownOf(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackHole.blackholeUnlocked)
            SetCooldownOf(blackholeImage);


        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.Instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackHole.cooldown);

        CheckCooldownOf(flaskImage, Inventory.Instance.flaskCooldown);

    }


    private void UpdateSoulsUI()
    {
        if (soulsAmount < PlayerManager.Instance.GetCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.Instance.GetCurrency();


        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    // 这里只是通过UI告诉玩家CD情况，实际能否使用跟这里并没有多大关系，相当于这里仅仅是个可视化的技能计时器，真实的技能计时器还是技能本身控制
    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }

}
