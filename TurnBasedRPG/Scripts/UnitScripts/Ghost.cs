using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Unit
{



    public override void Buff()
    {
        SetIsBlinded(true);
        Strength += 2;
    }

    public override void Debuff(Unit unit)
    {
        unit.SpendMana(Strength);
    }

    public override string AttackNameOne()
    {
        return " jumps on you!!";
    }

    public override string AttackNameTwo()
    {
        return " goes into a blind rage";
    }
    public override string AttackNameThree()
    {
        return " drains your spirit";
    }
}
