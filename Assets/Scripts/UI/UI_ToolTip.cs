using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    // 根据鼠标位置调整提示框，但是没必要，没使用。真要使用的话，应该获取屏幕大小自动调整，而不是固定的
    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        float newXoffset = 0;
        float newYoffset = 0;

        if (mousePosition.x > xLimit)
            newXoffset = -xOffset;
        else
            newXoffset = xOffset;

        if (mousePosition.y > yLimit)
            newYoffset = -yOffset;
        else
            newYoffset = yOffset;

        transform.position = new Vector2(mousePosition.x + newXoffset, mousePosition.y + newYoffset);
    }
    // 根据字符串长度修改字体大小，没必要，TMP自带调整字体大小功能
    public void AdjustFontSize(TextMeshProUGUI _text)
    {
        if (_text.text.Length > 12)
            _text.fontSize = _text.fontSize * .8f;
    }
}
