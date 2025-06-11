using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;

    public void ShowToolTip(string _skillDescprtion, string _skillName, int _price)
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return; // this hides tooltip if you hide left control

        skillName.text = _skillName;
        skillText.text = _skillDescprtion;
        skillCost.text = "Cost: " + _price;


        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
