// �浵�������ӿڣ���Ҫ����浵ϵͳ���������ʵ�ִ˽ӿ�
public interface ISaveManager
{
    // ��GameData��������
    void LoadData(GameData _data);
    // �����ݱ��浽GameData
    void SaveData(ref GameData _data);
}