using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Hero : PlayerCard
{
    public override void Setup()
    {
        logName = "a Hero";
        myColor = CardColor.Blue;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToDraw(2);
        if (currPlayer.listOfHand.Count <= 5)
            currPlayer.TryToGain(5);
        yield return null;
    }
}
