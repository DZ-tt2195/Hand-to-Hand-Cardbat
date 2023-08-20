using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unicorn : PlayerCard
{
    public override void Setup()
    {
        logName = "a Unicorn";
        myColor = CardColor.Red;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
