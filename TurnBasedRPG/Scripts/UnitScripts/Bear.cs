using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Unit
{

    public override string AttackNameOne()
    {
        return " lunges to bite you!";
    }

    public override string AttackNameTwo()
    {
        return " roars! attack up!";
    }
    public override string AttackNameThree()
    {
        return " claws you! You're bleeding!";
    }
}
