using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Warlord : PlayerCard
{
    public override void Setup()
    {
        logName = "a Warlord";
        myColor = CardColor.Gold;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.pv.RPC("RequestDraw", RpcTarget.MasterClient, 2);
        yield return new WaitForSeconds(0.5f);

        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i< currPlayer.listOfHand.Count; i++)
        {
            if (currPlayer.listOfHand[i].myCost == 10 && currPlayer.coins >= currPlayer.listOfHand[i].myCost)
                possibleCards.Add(currPlayer.listOfHand[i]);
        }
        yield return currPlayer.ChooseToPlay(possibleCards, "Warlord");
    }
}
