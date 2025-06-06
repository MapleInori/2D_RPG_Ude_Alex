using UnityEngine;


public class ItemEffect : ScriptableObject
{
    public virtual void ExecuteEffect(Transform _targetTrans)
    {
        Debug.Log("Effect Executed.");
    }
}
