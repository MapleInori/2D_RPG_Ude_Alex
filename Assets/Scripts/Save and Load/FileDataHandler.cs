using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 文件数据处理器类，负责游戏数据的实际文件读写操作
public class FileDataHandler
{
    private string dataDirPath = "";   // 存档文件目录路径
    private string dataFileName = "";   // 存档文件名
    private bool encryptData = false;   // 是否加密数据标志
    private string codeWord = "MapleInori"; // 加密使用的密钥

    // 构造函数，初始化文件处理器
    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;      // 设置存档目录
        dataFileName = _dataFileName;    // 设置存档文件名
        encryptData = _encryptData;     // 设置是否加密
    }

    // 保存游戏数据到文件
    public void Save(GameData _data)
    {
        // 组合完整文件路径
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // 确保目录存在，不存在则创建
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 将游戏数据转换为JSON格式字符串
            string dataToStore = JsonUtility.ToJson(_data, true);

            // 如果需要加密数据
            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            // 使用文件流写入数据
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);  // 写入数据到文件
                }
            }
        }
        catch (Exception e)  // 捕获并处理异常
        {
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    // 从文件加载游戏数据
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;  // 初始化返回数据

        // 检查文件是否存在
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                // 读取文件内容
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();  // 读取全部内容
                    }
                }

                // 如果需要解密数据
                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                // 将JSON字符串反序列化为GameData对象
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)  // 捕获并处理异常
            {
                Debug.LogError("Error on trying to load data from file:" + fullPath + "\n" + e);
            }
        }

        return loadData;  // 返回加载的数据(可能为null)
    }

    // 删除存档文件
    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        // 如果文件存在则删除
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    // 简单的加密/解密方法(异或加密)
    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";

        // 对每个字符进行异或运算
        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}