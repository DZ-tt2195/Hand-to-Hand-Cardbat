using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Magpie : PlayerCard
{
    public override void Setup()
    {
        logName = "a Magpie";
        myColor = CardColor.Gold;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
