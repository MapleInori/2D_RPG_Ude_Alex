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

    // TODO�����Է����仯ʱ����Ҫ��鵱ǰ����ֵȻ���ж��Ƿ�����޸ģ���Ӧ��ֻ�������˺ͻ�Ѫ��ʱ��ı�
    // Ŀǰ�����������ֵ1000������10����������£���ǰ����ֵ��1030

    private void UpdateCurrentHealthUI()
    {
        currentHealthText.text = playerStats.currentHealth.ToString();
    }
}
