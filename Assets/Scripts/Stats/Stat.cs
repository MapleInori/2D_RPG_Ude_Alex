using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField]private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int item in modifiers)
        {
            finalValue += item;
        }

        return finalValue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifer)
    {
        modifiers.Add(_modifer);
    }

    public void RemoveModifier(int _modifer)
    {
        modifiers.Remove(_modifer);
    }
}
