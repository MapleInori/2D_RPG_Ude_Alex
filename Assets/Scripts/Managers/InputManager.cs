
using UnityEngine;

// TODO:待完善
public static class InputManager
{
    public static KeyCode SwordSkillKey = KeyCode.U;
    
}

/*
 * 
 * [2025-06-06 18:45:12] # 基于静态类的可配置按键管理方案

既然你想避免使用Unity的Input System，同时需要支持玩家自定义按键，这里提供一个更完善的静态类方案，包含按键修改和保存功能：

## 完整实现方案

```csharp
using System.Collections.Generic;
using UnityEngine;

public static class KeyConfig
{
    // 默认按键配置
    private static Dictionary<string, KeyCode> _keyBindings = new Dictionary<string, KeyCode>()
    {
        {"Jump", KeyCode.Space},
        {"Attack", KeyCode.Mouse0},
        {"Interact", KeyCode.E},
        {"Sprint", KeyCode.LeftShift},
        // 添加更多按键...
    };

    // 获取按键
    public static KeyCode GetKey(string actionName)
    {
        if (_keyBindings.TryGetValue(actionName, out KeyCode key))
        {
            return key;
        }
        Debug.LogWarning($"未找到动作{actionName}的按键绑定");
        return KeyCode.None;
    }

    // 修改按键绑定
    public static void SetKey(string actionName, KeyCode newKey)
    {
        if (_keyBindings.ContainsKey(actionName))
        {
            _keyBindings[actionName] = newKey;
            SaveKeys();
        }
    }

    // 保存到PlayerPrefs
    private static void SaveKeys()
    {
        foreach (var pair in _keyBindings)
        {
            PlayerPrefs.SetInt("Key_" + pair.Key, (int)pair.Value);
        }
        PlayerPrefs.Save();
    }

    // 从PlayerPrefs加载
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

    // 重置为默认按键
    public static void ResetToDefault()
    {
        _keyBindings = new Dictionary<string, KeyCode>()
        {
            {"Jump", KeyCode.Space},
            {"Attack", KeyCode.Mouse0},
            {"Interact", KeyCode.E},
            {"Sprint", KeyCode.LeftShift},
            // 默认配置...
        };
        SaveKeys();
    }
}
```

## 使用方法

1. **初始化时加载配置**（在游戏启动时调用）：
```csharp
KeyConfig.LoadKeys();
```

2. **在游戏代码中检测按键**：
```csharp
if(Input.GetKey(KeyConfig.GetKey("Jump"))) 
{
    // 跳跃逻辑
}
```

3. **在设置界面修改按键**：
```csharp
// 当玩家选择新按键时
KeyConfig.SetKey("Jump", selectedKey);
```

## 实现设置面板的建议

1. 为每个动作创建UI按钮
2. 点击按钮后进入"等待按键输入"状态
3. 检测玩家按下的新按键并调用`SetKey`

```csharp
// 示例：等待按键输入的协程
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

## 优势

1. **完全静态**：全局可访问，无需实例化
2. **持久化存储**：使用PlayerPrefs自动保存配置
3. **易于扩展**：添加新动作只需在字典中新增条目
4. **与Unity原生Input兼容**：继续使用熟悉的Input类

这个方案比纯静态常量更灵活，同时避免了Input System的复杂性，非常适合中小型项目的按键管理需求。
 */