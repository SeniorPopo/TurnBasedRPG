using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Unit
{
    public int Luck;
    public override void UpdatePrefab()
    {
        MaxHP = 10 + (Constitution * 5);
        CurrentHP = MaxHP;
        MaxMana = 10 + (UnitLevel * 5);
        CurrentMana = MaxMana;
        Damage = Intelligence * 5;
    }

    public void CriticalEye()
    {
        Luck *= 2;
    }
}
