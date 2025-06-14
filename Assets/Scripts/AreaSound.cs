using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 环境音效类：用于在特定区域内播放和停止音效，音效索引对应的音效建议打开Loop
/// </summary>
public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.Instance.PlaySFX(areaSoundIndex, null);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.Instance.StopSFXWithTime(areaSoundIndex);
    }
}
