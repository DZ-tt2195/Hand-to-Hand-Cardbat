using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Watchdog : PlayerCard
{
    public override void Setup()
    {
        logName = "a Watchdog";
        myColor = CardColor.Red;
        myCost = 0;
        myCrowns = 1;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
