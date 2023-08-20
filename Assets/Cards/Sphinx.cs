using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sphinx : PlayerCard
{
    public override void Setup()
    {
        logName = "a Sphinx";
        myColor = CardColor.Gold;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }
}
