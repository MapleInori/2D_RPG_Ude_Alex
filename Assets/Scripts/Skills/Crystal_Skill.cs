using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal Mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Explosive Crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseMultiStack;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCoolDown;
    [SerializeField] private float useTimeWindow;   // ʹ��ʱ�䴰�ڣ���ʱ����û�м���ʹ�������CD
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    public override void UseSkill()
    {
        base.UseSkill();
        if (CanUseMultiCrystal())
        {
            return;
        }
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMoveToEnemy) return;
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.Instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);

                Destroy(currentCrystal);
            }
            else
            {
                // ���������̱�ը������������У��Ǿ��ǵ�ʱ�䱬ը
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.ExplodeOrDestroy();

            }


        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform),player);

    }

    public void CurrentCrystalChooseRandomTarget()
    {
        currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy() ;
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStack)
        {
            // ��������ˮ��
            if (crystalLeft.Count > 0)
            {
                // ʹ�õ�һ��������ʼʹ�ü��ܣ���������CD��������ڸ�CD��ʹ����ϣ������CD
                if (crystalLeft.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }

                // ���ܹ�ʹ��ʱ��CD����Ϊ0�����ܹ����ʹ��ֱ��ˮ������
                cooldown = 0;

                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                // ʹ�ú����ˮ������Ϊ0��������CD���������ˮ��
                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCoolDown;
                    RefillCrystal();
                }
                return true;
            }


        }

        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0) return;

        cooldownTimer = multiStackCoolDown;
        RefillCrystal();
    }
}
