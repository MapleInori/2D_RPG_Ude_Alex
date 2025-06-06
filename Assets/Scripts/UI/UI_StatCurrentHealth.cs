using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatCurrentHealth : MonoBehaviour
{
    private PlayerStats playerStats;
    [SerializeField]private TextMeshProUGUI currentHealthText;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = PlayerManager.Instance.player.GetComponent<PlayerStats>();
        playerStats.onHealthChanged += UpdateCurrentHealthUI;
    }

    // TODO：属性发生变化时都需要检查当前生命值然后判断是否进行修改，不应该只是在受伤和回血的时候改变
    // 目前开局最大生命值1000，但是10体力的情况下，当前生命值是1030

    private void UpdateCurrentHealthUI()
    {
        currentHealthText.text = playerStats.currentHealth.ToString();
    }
}
