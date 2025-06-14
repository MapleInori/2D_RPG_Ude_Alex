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
        // 添加if判断，解决重新加载场景时报错：MissingReferenceException:
        // The object of type 'Animator' has been destroyed but you are still trying to access it.
        // Your script should either check if it is null or you should not destroy the object.
        // 可能在取消DontDestroyOnLoad的时候这里就没问题了，不过保留这个判断无伤大雅
        if (anim != null)
            anim.SetBool("Active", true);
    }
}
