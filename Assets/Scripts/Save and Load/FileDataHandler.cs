using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// �ļ����ݴ������࣬������Ϸ���ݵ�ʵ���ļ���д����
public class FileDataHandler
{
    private string dataDirPath = "";   // �浵�ļ�Ŀ¼·��
    private string dataFileName = "";   // �浵�ļ���
    private bool encryptData = false;   // �Ƿ�������ݱ�־
    private string codeWord = "MapleInori"; // ����ʹ�õ���Կ

    // ���캯������ʼ���ļ�������
    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;      // ���ô浵Ŀ¼
        dataFileName = _dataFileName;    // ���ô浵�ļ���
        encryptData = _encryptData;     // �����Ƿ����
    }

    // ������Ϸ���ݵ��ļ�
    public void Save(GameData _data)
    {
        // ��������ļ�·��
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // ȷ��Ŀ¼���ڣ��������򴴽�
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // ����Ϸ����ת��ΪJSON��ʽ�ַ���
            string dataToStore = JsonUtility.ToJson(_data, true);

            // �����Ҫ��������
            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            // ʹ���ļ���д������
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);  // д�����ݵ��ļ�
                }
            }
        }
        catch (Exception e)  // ���񲢴����쳣
        {
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    // ���ļ�������Ϸ����
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;  // ��ʼ����������

        // ����ļ��Ƿ����
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                // ��ȡ�ļ�����
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();  // ��ȡȫ������
                    }
                }

                // �����Ҫ��������
                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                // ��JSON�ַ��������л�ΪGameData����
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)  // ���񲢴����쳣
            {
                Debug.LogError("Error on trying to load data from file:" + fullPath + "\n" + e);
            }
        }

        return loadData;  // ���ؼ��ص�����(����Ϊnull)
    }

    // ɾ���浵�ļ�
    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        // ����ļ�������ɾ��
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    // �򵥵ļ���/���ܷ���(������)
    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";

        // ��ÿ���ַ������������
        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}