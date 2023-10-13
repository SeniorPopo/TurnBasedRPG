using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Unit
{



    public override void Buff()
    {
        return;
    }
    public override void Debuff(Unit unit)
    {
        unit.LowerDamage(-2);
    }

    public override string AttackNameOne()
    {
        return " jumps on you!!";
    }

    public override string AttackNameTwo()
    {
        return " hops around joyfully";
    }
    public override string AttackNameThree()
    {
        return " gives you a sad look, attack down!";
    }
}
