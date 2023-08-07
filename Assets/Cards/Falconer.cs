using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Falconer : PlayerCard
{
    public override void Setup()
    {
        logName = "a Falconer";
        myColor = CardColor.Blue;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToGain(5);
        yield return null;
    }

}
