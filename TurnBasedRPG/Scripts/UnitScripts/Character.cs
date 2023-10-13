using UnityEngine;

public class Character : MonoBehaviour
{
    public string UnitName;
    public CharacterStat Strength;
    public CharacterStat UnitLevel;
    public CharacterStat Intelligence;
    public CharacterStat Constitution;
    public CharacterStat Dexterity;
    public CharacterStat MaxHP;
    // public int CurrentHP;
    public CharacterStat MaxMana;
    // public int CurrentMana;
    public CharacterStat EXP;
    //public int MaxExp;
    public bool IsBleeding;
    public bool IsBlinded;
}

public class Item
{
    public void Equip(Character c)
    {
        // We need to store our modifiers in variables before adding them to the stat
        c.Strength.AddModifier(new StatModifier(10, StatModType.Flat, this));
        c.Strength.AddModifier(new StatModifier(0.1f, StatModType.PercentMult, this));
    }

    public void Unequip(Character c) 
    {
        c.Strength.RemoveAllModifiersFromSource(this);
    }
}