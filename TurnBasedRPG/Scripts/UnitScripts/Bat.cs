using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Unit
{



    public override void Buff()
    {
        return;
    }
    public override void Debuff(Unit unit)
    {
        unit.SetIsBlinded(true);
    }

    public override string AttackNameOne()
    {
        return " dives at you!";
    }

    public override string AttackNameTwo()
    {
        return " flies around scared";
    }
    public override string AttackNameThree()
    {
        return " let's out a hypersonic scream, accuracy down!";
    }
}
