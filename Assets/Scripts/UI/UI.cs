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


    // TODO:toolTip都改为TipPanel？
    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        SwitchTo(skillTreeUI); // we need this to assign events on skill tree slots before we asssign events on skill scripts
        
        // 打开它时会让画面淡入，这里跟从主菜单进入到游戏的淡入效果重叠，不影响实际效果，主要是避免开发时黑屏遮挡，开发时可以将它关闭
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
        // 如果是打开的界面，再按一下关掉
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
            // 只要有一个UI处于显示状态，就不用显示战斗时UI，排除负责渐入渐出效果的UI
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }

    /// <summary>
    /// 画面渐出，显示结束界面
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
        // 从数据里找到音量设置，<参数名，音量值>
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            // 从UI上找到对应的音量滑块脚本
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                // 匹配名称，设置参数
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
