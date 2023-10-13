using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public string UnitName;
    public int UnitLevel;
    public int Intelligence;
    public int Strength;
    public int Constitution;
    public int Dexterity;
    protected int Damage;
    protected int MaxHP;
    protected int CurrentHP;
    protected int MaxMana;
    protected int CurrentMana;
    protected int CurrentExp;
    protected int MaxExp;
    protected bool IsBleeding;
    protected bool IsBlinded;

    public string GetUnitName()
    {
        return UnitName;
    }

    public int GetUnitLevel()
    {
        return UnitLevel;
    }

    public int GetMaxHP()
    {
        return MaxHP;
    }

    public int GetMaxMana()
    {
        return MaxMana;
    }
    public int GetMaxExp()
    {
        return MaxExp;
    }

    public int GetCurrentHP()
    {
        return CurrentHP;
    }

    public int GetCurrentMana()
    {
        return CurrentMana;
    }

    public int GetCurrentExp()
    {
        return CurrentExp;
    }

    public bool GetIsBleeding()
    {
        return IsBleeding;
    }

    public bool GetIsBlinded()
    {
        return IsBlinded;
    }

    public void SpendMana(int Mana)
    {
        CurrentMana -= Mana;
    }
    
    public void GainMana(int Mana)
    {
        CurrentMana += Mana;
    }

    public void SetCurrentExp(int Exp)
    {
        CurrentExp = Exp;
        if (CurrentExp > MaxExp)
        {
            UnitLevel++;
            CurrentExp = 0;
        }
    }

    public void SetIsBleeding(bool value)
    {
        IsBleeding = value;
    }

    public void SetIsBlinded(bool value)
    {
        IsBlinded = value; 
    }

    public void LowerDamage(int damage)
    {
        Damage -= damage;
    }

    public virtual void UpdatePrefab()
    {
        MaxHP = 10 + (Constitution * 5);
        CurrentHP = MaxHP;
        MaxMana = 10 + (UnitLevel * 5);
        CurrentMana = MaxMana;
        Damage = Strength * 5;
    }


    public bool TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (CurrentHP <= 0)
            return true;
        else
            return false;
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP >= MaxHP)
            CurrentHP = MaxHP;

    }

    public virtual int DealDamage()
    {
        return Damage;
    }


    public virtual void Buff()
    {
        Damage = (int)(Damage * 1.5);
    }

    public virtual void Debuff(Unit unit)
    {
        unit.IsBleeding = true;
    }


    public virtual string AttackNameOne()
    {
        return " attacks!";
    }

    public virtual string AttackNameTwo()
    {
        return " charges up!";
    }

    public virtual string AttackNameThree()
    {
        return " heals!";
    }
}
