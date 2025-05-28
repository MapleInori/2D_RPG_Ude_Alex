using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform myEnemyTrans;
    private BlackHole_Skill_Controller blackHole;

    public void SetupHotKey(KeyCode _myHotKey,Transform _myEnemy,BlackHole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemyTrans = _myEnemy;
        blackHole = _myBlackHole;

        myHotKey = _myHotKey;
        myText.text = _myHotKey.ToString();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO:��ʹ���ڼ����ڼ����ɿ��Դ�������û�޸�.Ӧ���ڵ���������ȼ�
        if (Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemyTrans);

            myText.color = Color.clear;
            sr.color = Color.clear;
            Debug.Log(myHotKey);
        }


    }
}
