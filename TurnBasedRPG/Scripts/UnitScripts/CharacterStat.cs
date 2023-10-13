using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CharacterStat
{
    public float BaseValue;

    public virtual float Value
    {
        get
        {
            if (isNotCaculated)
            {
                lastBaseValue = BaseValue;
                currentValue = CalculateFinalValue();
                isNotCaculated = false;
            }
            return currentValue;
        }
    }
    //checks to see if currentValue:(current stat value) needs to be re-calculated
    protected bool isNotCaculated = true;
    //holds most recent calculation
    protected float currentValue;
    protected float lastBaseValue = float.MinValue;
    //read only requires you be in the constructor or declaration to make changes. Meaning stat modifications must be Added , Removed, or Modified within the List. Preventing this list from being overwritten or nulled
    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;
   

    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }
    public CharacterStat(float baseValue) : this()
    {
        BaseValue = baseValue;
    }
    public virtual void AddModifier(StatModifier mod)
    {
        isNotCaculated = true;
        statModifiers.Add(mod);
        statModifiers.Sort();
    }

    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
        {
            return -1;
        }
        else if (a.Order > b.Order) 
        {
            return 1;
        }
        else 
        { 
            return 0;
        }
    }

    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isNotCaculated = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isNotCaculated = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        // using for loop because list values are modified
        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += statModifiers[i].Value;
            }
            else if (mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;

                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                {

                }
            }
            else if (mod.Type == StatModType.PercentMult)
            {
                //example formula: 1 + mod.Value(0.1) = 1.1 = 110%  (increase by 10%)
                finalValue *= 1 + mod.Value;
            }
        }

        //Will round to 4th zero .0001 to catch floats caused by % modifiers.
        return (float)Math.Round(finalValue,4);
    }
}
