using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    //[SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statType.ToString();

        if (statNameText != null)
        {
            statNameText.text = statType.ToString();
        }
    }

    private void Start()
    {
        UpdateStatValueUI();
        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.Instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();

            if (statType == StatType.Health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();

            if (statType == StatType.Damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.CritPower)
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue() * 0.1f).ToString() + "%";

            if (statType == StatType.CritChance)
                statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue() * 0.1f).ToString() + "%";

            if (statType == StatType.Evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue() * 0.1f).ToString() + "%";

            if (statType == StatType.MagicResistance)
                statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() * 3)).ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }
}
