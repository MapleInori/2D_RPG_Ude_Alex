// 存档管理器接口，需要参与存档系统的组件必须实现此接口
public interface ISaveManager
{
    // 从GameData加载数据
    void LoadData(GameData _data);
    // 将数据保存到GameData
    void SaveData(ref GameData _data);
}