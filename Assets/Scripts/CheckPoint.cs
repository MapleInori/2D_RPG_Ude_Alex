using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool activationStatus;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        if (activationStatus == false)
            AudioManager.Instance.PlaySFX(5, transform);


        activationStatus = true;
        // ���if�жϣ�������¼��س���ʱ����MissingReferenceException:
        // The object of type 'Animator' has been destroyed but you are still trying to access it.
        // Your script should either check if it is null or you should not destroy the object.
        // ������ȡ��DontDestroyOnLoad��ʱ�������û�����ˣ�������������ж����˴���
        if (anim != null)
            anim.SetBool("Active", true);
    }
}
