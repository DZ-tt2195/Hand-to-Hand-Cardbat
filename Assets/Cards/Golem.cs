using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Golem : PlayerCard
{
    public override void Setup()
    {
        logName = "a Golem";
        myColor = CardColor.Gold;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
