using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    // 音量控制滑块
    public Slider slider;

    // 混音器参数名：对应AudioMixer中暴露的音量参数（如"MasterVolume", "MusicVolume"）
    public string parametr;

    // 音频混音器
    [SerializeField] private AudioMixer audioMixer;

    // 音量转换系数：用于线性音量值到对数分贝值的转换
    [SerializeField] private float multiplier;

    // TODO:笔记：在按钮上添加事件监听时，记得选动态参数，而非静态参数。
    // 同时限制slider的最小值为0.001，不然当为0时，音量会回到0db
    public void SliderValue(float _value) => audioMixer.SetFloat(parametr, Mathf.Log10(_value) * multiplier);

    public void LoadSlider(float _value)
    {
        if (_value >= 0.001f)
            slider.value = _value;
    }
}
