using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Mastermind : PlayerCard
{
    public override void Setup()
    {
        logName = "a Mastermind";
        myColor = CardColor.Blue;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        List<PlayerCard> cardsOf0 = new List<PlayerCard>();
        List<PlayerCard> cardsOf5 = new List<PlayerCard>();
        List<PlayerCard> cardsOf10 = new List<PlayerCard>();

        for (int i = 0; i < currPlayer.listOfHand.Count; i++)
        {
            if (currPlayer.listOfHand[i].myCost == 0 && currPlayer.coins >= currPlayer.listOfHand[i].myCost)
                cardsOf0.Add(currPlayer.listOfHand[i]);
            else if (currPlayer.listOfHand[i].myCost == 5 && currPlayer.coins >= currPlayer.listOfHand[i].myCost)
                cardsOf5.Add(currPlayer.listOfHand[i]);
            else if (currPlayer.listOfHand[i].myCost == 10 && currPlayer.coins >= currPlayer.listOfHand[i].myCost)
                cardsOf10.Add(currPlayer.listOfHand[i]);
        }

        yield return currPlayer.ChooseToPlay(cardsOf0, "Mastermind");
        yield return currPlayer.ChooseToPlay(cardsOf5, "Mastermind");
        yield return currPlayer.ChooseToPlay(cardsOf10, "Mastermind");
    }
}
