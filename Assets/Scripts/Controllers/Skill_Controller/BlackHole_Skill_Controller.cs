using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackHoleTimer;

    private bool canGrow = true;
    private bool canCreatHotKeys = true;
    private bool canShrink;
    private bool cloneAttackReleased;
    private bool playerCanDisapear = true;

    private int amountOfAttack;
    private float cloneAttackCoolDown = 0.3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();    // 命中的敌人列表
    private List<GameObject> createdHotKey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttack, float _cloneAttackCoolDown, float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttack = _amountOfAttack;
        cloneAttackCoolDown = _cloneAttackCoolDown;
        blackHoleTimer = _blackHoleDuration;

        if (SkillManager.Instance.clone.crystalInsteadOfClone)
        {
            playerCanDisapear = false;
        }
    }

    private void Update()
    {
        blackHoleTimer -= Time.deltaTime;
        cloneAttackTimer -= Time.deltaTime;

        if (blackHoleTimer < 0)
        {
            // 避免在被销毁前反复执行
            blackHoleTimer = Mathf.Infinity;
            if (targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHoleAbility();
            }
        }
        // 释放攻击
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        }
        else if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0) return;

        // TODO:在这里销毁的话，无法避免技能结束前热键反复点击，频繁将添加过的敌人添加到敌人列表，导致攻击它的概率增高
        DestroyHotKey();
        cloneAttackReleased = true;
        canCreatHotKeys = false;

        if (playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.Instance.player.fx.MakeTransparent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer <= 0 && cloneAttackReleased && amountOfAttack > 0)
        {
            cloneAttackTimer = cloneAttackCoolDown;

            int randomIndex = Random.Range(0, targets.Count);

            // 镜像生成位置偏移
            float xOffset;
            if (Random.Range(1, 101) > 50)
            {
                xOffset = 1;
            }
            else
            {
                xOffset = -1;
            }

            if (SkillManager.Instance.clone.crystalInsteadOfClone)
            {
                SkillManager.Instance.crystal.CreateCrystal();
                SkillManager.Instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                // TODO：可能无法攻击到所有敌人
                SkillManager.Instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));

            }

            amountOfAttack--;
            //Debug.Log(amountOfAttack);
            // 攻击结束
            if (amountOfAttack <= 0)
            {
                Invoke("FinishBlackHoleAbility", 0.5f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKey();
        playerCanExitState = true;
        //PlayerManager.Instance.player.ExitBlackHoleAbility();
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.GetComponent<Enemy>() != null))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.FreezeTime(true);

            CreatHotKey(collision);



        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.GetComponent<Enemy>() != null))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.FreezeTime(false);
        }
    }

    private void DestroyHotKey()
    {
        if (createdHotKey.Count <= 0) return;

        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    /// <summary>
    /// 给命中的敌人添加一个触发黑洞技能伤害的热键
    /// </summary>
    /// <param name="collision"></param>
    private void CreatHotKey(Collider2D collision)
    {
        // 改成只用一个键就不用限制了，目前这样技能有命中上限，改了之后这里可以删除
        if (keyCodeList.Count < 0) return;

        if (!canCreatHotKeys) return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        createdHotKey.Add(newHotKey);


        //TODO:一个键就行了，没必要随机，不然的话按键数量决定了这个技能能命中的敌人数量，E？
        //或者不再移除按键，多个按键随机出现，最多同时出现几个，用过后其他敌人头上会重新使用按键
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);


        BlackHole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<BlackHole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }


    public void AddEnemyToList(Transform _enemyTransform)
    {
        targets.Add(_enemyTransform);
    }
}
