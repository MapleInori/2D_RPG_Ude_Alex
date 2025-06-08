using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��¡����
/// </summary>
public class Clone_Skill : Skill
{
    [SerializeField] private bool canAttack; // �Ƿ���Թ���
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;   // ����ʱ��
    [SerializeField] private float cloneFadeTime;   // ��ʧ����ʱ��

    [Header("Clone can Duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
    public bool crystalInsteadOfClone;


    public void CreateClone(Transform _cloneTransform, Vector3 _offset)
    {
        if(crystalInsteadOfClone)
        {
            SkillManager.Instance.crystal.CreateCrystal();

            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(
            _cloneTransform, cloneDuration, cloneFadeTime, canAttack, _offset, 
            FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate,player);
    }

    public void CreateCloneWithDealy(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector2(1.5f * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _Transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.3f);
        CreateClone(_Transform, _offset);
    }
}
