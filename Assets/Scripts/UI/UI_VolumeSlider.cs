using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    // �������ƻ���
    public Slider slider;

    // ����������������ӦAudioMixer�б�¶��������������"MasterVolume", "MusicVolume"��
    public string parametr;

    // ��Ƶ������
    [SerializeField] private AudioMixer audioMixer;

    // ����ת��ϵ����������������ֵ�������ֱ�ֵ��ת��
    [SerializeField] private float multiplier;

    // TODO:�ʼǣ��ڰ�ť������¼�����ʱ���ǵ�ѡ��̬���������Ǿ�̬������
    // ͬʱ����slider����СֵΪ0.001����Ȼ��Ϊ0ʱ��������ص�0db
    public void SliderValue(float _value) => audioMixer.SetFloat(parametr, Mathf.Log10(_value) * multiplier);

    public void LoadSlider(float _value)
    {
        if (_value >= 0.001f)
            slider.value = _value;
    }
}
