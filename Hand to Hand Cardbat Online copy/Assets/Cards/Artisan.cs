using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Artisan : PlayerCard
{
    public override void Setup()
    {
        logName = "an Artisan";
        myColor = CardColor.Red;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        List<PlayerCard> currentHand = currPlayer.listOfHand;
        for (int i = 0; i<currentHand.Count; i++)
        {
            yield return currentHand[i].DiscardEffect(currPlayer, 1);
        }

        currPlayer.TryToDraw(5);
    }

}
