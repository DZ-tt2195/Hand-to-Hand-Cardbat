using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Advisor : PlayerCard
{
    public override void Setup()
    {
        logName = "an Advisor";
        myColor = CardColor.Blue;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
