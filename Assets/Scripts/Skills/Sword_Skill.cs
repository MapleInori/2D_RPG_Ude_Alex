
using System;
using UnityEngine;
using UnityEngine.UI;


public enum SwordType
{
    Normal,// 普通
    Bounce,// 弹射
    Pierce,// 穿刺
    Spin,  // 旋转
}
/// <summary>
/// 扔剑技能，通过WS瞄准。原教程使用鼠标，并且允许翻转，我不要了。
/// </summary>
public class Sword_Skill : Skill
{
    [SerializeField] private SwordType swordType = SwordType.Normal;

    [Header("Skill Info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float swordGravity;
    [SerializeField] private float delayDestroyTime;
    [SerializeField] private float normalGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool vulnerableUnlocked { get; private set; }

    [Header("Bounce Info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce Info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCoolDown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;


    [Header("Aim Info")]
    public float throwAngle = 0f; // 当前角度
    public float angleSpeed = 90f; // 角度变化速度（度/秒）
    public float minAngle = -30f; // 最小角度
    public float maxAngle = 80f;  // 最大角度

    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotsPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    private Vector2 finalDir;

    private SwordType lastSwordType;    // 暂时没用了，可以删除

    protected override void Start()
    {
        base.Start();
        GenerateDots();
        SetupGravity();

        // 获取技能解锁按钮，监听解锁情况
        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnurable);
    }

    #region 技能解锁区域
    // TODO：如果有重置技能的道具，解锁方法就应该添加else，当未解锁时重新锁定技能，在重置技能后调用CheckUnlock方法。读档的时候默认未解锁，所以直接检查即可。
    //protected override void CheckUnlock()
    //{
    //    UnlockSword();
    //    UnlockBounceSword();
    //    UnlockSpinSword();
    //    UnlockPierceSword();
    //    UnlockTimeStop();
    //    UnlockVulnurable();
    //}
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }

    private void UnlockVulnurable()
    {
        if (vulnerableUnlockButton.unlocked)
            vulnerableUnlocked = true;
    }

    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Normal;
            swordUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }



    #endregion

    private void SetupGravity()
    {
        if(swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        if(swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        if(swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.U))
        {
            switch(swordType)
            {
                case SwordType.Normal:swordGravity = normalGravity;break;
                case SwordType.Bounce:swordGravity = bounceGravity;break;
                case SwordType.Pierce:swordGravity = pierceGravity;break;
                case SwordType.Spin:swordGravity = spinGravity;break;
            }
            // 根据输入调整角度
            float input = Input.GetAxis("Vertical");
            throwAngle += input * angleSpeed * Time.deltaTime;
            throwAngle = Mathf.Clamp(throwAngle, minAngle, maxAngle);

            AimDirection();
            UpdateDotsPositions();
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordSkillController = newSword.GetComponent<Sword_Skill_Controller>();

        if(swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
            lastSwordType = swordType;
            newSwordSkillController.SetupBounce(true,bounceAmount,bounceSpeed);
        }
        else if(swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
            lastSwordType = swordType;
            newSwordSkillController.SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
            lastSwordType = swordType;
            newSwordSkillController.SetupSpin(true,maxTravelDistance,spinDuration,hitCoolDown);
        }

        newSwordSkillController.SetupSword(finalDir.normalized * launchSpeed, swordGravity, player, delayDestroyTime,freezeTimeDuration,returnSpeed);
        player.AssignNewSword(newSword);
        DotsActive(false);
    }

    #region Aim  
    private void AimDirection()
    {
        // 计算最终方向（包含面朝方向修正）
        float rad = throwAngle * Mathf.Deg2Rad;
        finalDir = new Vector2(
            Mathf.Cos(rad) * player.facingDir, // 水平方向根据面朝方向调整
            Mathf.Sin(rad)
        );
        //Debug.Log(finalDir);
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotsPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        // 草，位移公式，速度乘时间 + 1/2Gt^2.前一半计算无重力位移，后一半是是重力所作的位移
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            finalDir.normalized.x * launchSpeed,
            finalDir.normalized.y * launchSpeed) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    private void UpdateDotsPositions()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
        }
    }
    #endregion 
}
