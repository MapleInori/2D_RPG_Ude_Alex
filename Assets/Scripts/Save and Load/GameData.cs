using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏数据类，用于存储和管理游戏的各种状态和进度数据
// 标记为可序列化，以便可以保存到文件或在Unity中查看
[System.Serializable]
public class GameData
{
    // 玩家当前拥有的货币数量
    public int currency;

    // 技能树状态字典：键是技能ID，值表示是否已解锁（true/false）
    public SerializableDictionary<string, bool> skillTree;

    // 物品库存字典：键是物品ID，值是该物品的数量
    public SerializableDictionary<string, int> inventory;

    // 当前装备的物品ID列表
    public List<string> equipmentId;

    // 检查点状态字典：键是检查点ID，值表示是否已激活（true/false）
    public SerializableDictionary<string, bool> checkpoints;

    // 最近激活的检查点ID
    public string closestCheckpointId;

    // 玩家死亡时丢失货币的位置X坐标
    public float lostCurrencyX;
    // 玩家死亡时丢失货币的位置Y坐标
    public float lostCurrencyY;
    // 玩家死亡时丢失的货币数量
    public int lostCurrencyAmount;

    // 音量设置字典：键是音频类型（如"Master"、"Music"等），值是音量大小（0-1）
    public SerializableDictionary<string, float> volumeSettings;

    // 构造函数，初始化所有数据字段
    public GameData()
    {
        // 初始化丢失货币相关数据
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        // 初始化基础数据
        this.currency = 0;

        // 初始化各种字典和列表
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        // 初始化检查点相关数据
        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();

        // 初始化音量设置
        volumeSettings = new SerializableDictionary<string, float>();
    }
}