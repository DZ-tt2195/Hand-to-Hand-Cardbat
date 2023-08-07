using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Adventurer : PlayerCard
{
    public override void Setup()
    {
        logName = "an Adventurer";
        myColor = CardColor.Blue;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToDraw(2);
        if (currPlayer.listOfHand.Count <= 4)
            currPlayer.TryToGain(5);
        yield return null;
    }
}
