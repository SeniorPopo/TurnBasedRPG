using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Unit
{

    private bool MinionSummoned;


    public override void UpdatePrefab()
    {
        MaxHP = 10 + (Constitution * 5);
        CurrentHP = MaxHP;
        MaxMana = 10 + (UnitLevel * 5);
        CurrentMana = MaxMana;
        Damage = Intelligence * 5;
    }

    public bool GetMinionSummoned()
    {
        return MinionSummoned;
    }

    public void SummonSkeleton()
    {
        MinionSummoned = true;
    }

    


}
