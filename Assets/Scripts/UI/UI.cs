using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager 
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject charcaterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;


    // TODO:toolTip����ΪTipPanel��
    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        SwitchTo(skillTreeUI); // we need this to assign events on skill tree slots before we asssign events on skill scripts
        
        // ����ʱ���û��浭�룬����������˵����뵽��Ϸ�ĵ���Ч���ص�����Ӱ��ʵ��Ч������Ҫ�Ǳ��⿪��ʱ�����ڵ�������ʱ���Խ����ر�
        fadeScreen.gameObject.SetActive(true);  
    }

    void Start()
    {
        SwitchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);

        //gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(charcaterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);


        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchWithKeyTo(optionsUI);


    }


    public void SwitchTo(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null; // we need this to keep fade screen game object active


            if (fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }
        if(_menu !=null)
        {
            AudioManager.Instance.PlaySFX(7,null);
            _menu.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            if (_menu == inGameUI)
                GameManager.Instance.PauseGame(false);
            else
                GameManager.Instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        // ����Ǵ򿪵Ľ��棬�ٰ�һ�¹ص�
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            // ֻҪ��һ��UI������ʾ״̬���Ͳ�����ʾս��ʱUI���ų������뽥��Ч����UI
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }

    /// <summary>
    /// ���潥������ʾ��������
    /// </summary>
    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);

    }

    public void RestartGameButton() => GameManager.Instance.RestartScene();

    public void LoadData(GameData _data)
    {
        // ���������ҵ��������ã�<������������ֵ>
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            // ��UI���ҵ���Ӧ����������ű�
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                // ƥ�����ƣ����ò���
                if (item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
