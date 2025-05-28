
using System;
using UnityEngine;


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
    [Header("Bounce Info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce Info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField] private float hitCoolDown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    [Header("Skill Info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchDir;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float swordGravity;
    [SerializeField] private float delayDestroyTime;
    [SerializeField] private float normalGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

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

    private Transform swordTrans;

    private SwordType lastSwordType;

    protected override void Start()
    {
        base.Start();
        GenerateDots();
        SetupGravity();
    }

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
            Mathf.Cos(rad) * player.faceDir, // ˮƽ��������泯�������
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
