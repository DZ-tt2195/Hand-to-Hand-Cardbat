using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spy : PlayerCard
{
    public override void Setup()
    {
        logName = "a Spy";
        myColor = CardColor.Blue;
        myCost = 0;
        myCrowns = 1;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
