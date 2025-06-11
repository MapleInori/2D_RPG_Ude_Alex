
using System;
using UnityEngine;
using UnityEngine.UI;


public enum SwordType
{
    Normal,// ��ͨ
    Bounce,// ����
    Pierce,// ����
    Spin,  // ��ת
}
/// <summary>
/// �ӽ����ܣ�ͨ��WS��׼��ԭ�̳�ʹ����꣬��������ת���Ҳ�Ҫ�ˡ�
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
    public float throwAngle = 0f; // ��ǰ�Ƕ�
    public float angleSpeed = 90f; // �Ƕȱ仯�ٶȣ���/�룩
    public float minAngle = -30f; // ��С�Ƕ�
    public float maxAngle = 80f;  // ���Ƕ�

    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotsPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    private Vector2 finalDir;

    private SwordType lastSwordType;    // ��ʱû���ˣ�����ɾ��

    protected override void Start()
    {
        base.Start();
        GenerateDots();
        SetupGravity();

        // ��ȡ���ܽ�����ť�������������
        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnurable);
    }

    #region ���ܽ�������
    // TODO����������ü��ܵĵ��ߣ�����������Ӧ�����else����δ����ʱ�����������ܣ������ü��ܺ����CheckUnlock������������ʱ��Ĭ��δ����������ֱ�Ӽ�鼴�ɡ�
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
            // ������������Ƕ�
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
        // �������շ��򣨰����泯����������
        float rad = throwAngle * Mathf.Deg2Rad;
        finalDir = new Vector2(
            Mathf.Cos(rad) * player.facingDir, // ˮƽ��������泯�������
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
        // �ݣ�λ�ƹ�ʽ���ٶȳ�ʱ�� + 1/2Gt^2.ǰһ�����������λ�ƣ���һ����������������λ��
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
