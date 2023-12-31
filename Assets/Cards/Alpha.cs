using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Alpha : PlayerCard
{
    public override void Setup()
    {
        logName = "an Alpha";
        myColor = CardColor.Red;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Alpha");

        yield return new WaitForSeconds(0.5f);
        if (currPlayer.cardsPlayedThisTurn.Count >= 3)
            currPlayer.TryToGain(10);
    }
}
