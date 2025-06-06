
using UnityEngine;

// TODO:������
public static class InputManager
{
    public static KeyCode SwordSkillKey = KeyCode.U;
    
}

/*
 * 
 * [2025-06-06 18:45:12] # ���ھ�̬��Ŀ����ð���������

��Ȼ�������ʹ��Unity��Input System��ͬʱ��Ҫ֧������Զ��尴���������ṩһ�������Ƶľ�̬�෽�������������޸ĺͱ��湦�ܣ�

## ����ʵ�ַ���

```csharp
using System.Collections.Generic;
using UnityEngine;

public static class KeyConfig
{
    // Ĭ�ϰ�������
    private static Dictionary<string, KeyCode> _keyBindings = new Dictionary<string, KeyCode>()
    {
        {"Jump", KeyCode.Space},
        {"Attack", KeyCode.Mouse0},
        {"Interact", KeyCode.E},
        {"Sprint", KeyCode.LeftShift},
        // ��Ӹ��ఴ��...
    };

    // ��ȡ����
    public static KeyCode GetKey(string actionName)
    {
        if (_keyBindings.TryGetValue(actionName, out KeyCode key))
        {
            return key;
        }
        Debug.LogWarning($"δ�ҵ�����{actionName}�İ�����");
        return KeyCode.None;
    }

    // �޸İ�����
    public static void SetKey(string actionName, KeyCode newKey)
    {
        if (_keyBindings.ContainsKey(actionName))
        {
            _keyBindings[actionName] = newKey;
            SaveKeys();
        }
    }

    // ���浽PlayerPrefs
    private static void SaveKeys()
    {
        foreach (var pair in _keyBindings)
        {
            PlayerPrefs.SetInt("Key_" + pair.Key, (int)pair.Value);
        }
        PlayerPrefs.Save();
    }

    // ��PlayerPrefs����
    public static void LoadKeys()
    {
        foreach (var key in _keyBindings.Keys)
        {
            if (PlayerPrefs.HasKey("Key_" + key))
            {
                _keyBindings[key] = (KeyCode)PlayerPrefs.GetInt("Key_" + key);
            }
        }
    }

    // ����ΪĬ�ϰ���
    public static void ResetToDefault()
    {
        _keyBindings = new Dictionary<string, KeyCode>()
        {
            {"Jump", KeyCode.Space},
            {"Attack", KeyCode.Mouse0},
            {"Interact", KeyCode.E},
            {"Sprint", KeyCode.LeftShift},
            // Ĭ������...
        };
        SaveKeys();
    }
}
```

## ʹ�÷���

1. **��ʼ��ʱ��������**������Ϸ����ʱ���ã���
```csharp
KeyConfig.LoadKeys();
```

2. **����Ϸ�����м�ⰴ��**��
```csharp
if(Input.GetKey(KeyConfig.GetKey("Jump"))) 
{
    // ��Ծ�߼�
}
```

3. **�����ý����޸İ���**��
```csharp
// �����ѡ���°���ʱ
KeyConfig.SetKey("Jump", selectedKey);
```

## ʵ���������Ľ���

1. Ϊÿ����������UI��ť
2. �����ť�����"�ȴ���������"״̬
3. �����Ұ��µ��°���������`SetKey`

```csharp
// ʾ�����ȴ����������Э��
IEnumerator WaitForKeyPress(string actionName)
{
    yield return new WaitUntil(() => Input.anyKeyDown);
    
    foreach(KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
    {
        if(Input.GetKey(keyCode))
        {
            KeyConfig.SetKey(actionName, keyCode);
            break;
        }
    }
}
```

## ����

1. **��ȫ��̬**��ȫ�ֿɷ��ʣ�����ʵ����
2. **�־û��洢**��ʹ��PlayerPrefs�Զ���������
3. **������չ**������¶���ֻ�����ֵ���������Ŀ
4. **��Unityԭ��Input����**������ʹ����Ϥ��Input��

��������ȴ���̬��������ͬʱ������Input System�ĸ����ԣ��ǳ��ʺ���С����Ŀ�İ�����������
 */