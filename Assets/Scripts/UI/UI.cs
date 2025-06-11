using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("End screen")]
    //[SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject charcaterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;


    // TODO:toolTip都改为TipPanel
    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    //[SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        SwitchTo(skillTreeUI); // we need this to assign events on skill tree slots before we asssign events on skill scripts
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
            transform.GetChild(i).gameObject.SetActive(false);
        }
        if(_menu !=null)
        {
            _menu.SetActive(true);
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
            // 只要有一个UI处于显示状态，就不用显示战斗时UI
            if (transform.GetChild(i).gameObject.activeSelf /*&& transform.GetChild(i).GetComponent<UI_FadeScreen>() == null*/)
                return;
        }

        SwitchTo(inGameUI);
    }
}
