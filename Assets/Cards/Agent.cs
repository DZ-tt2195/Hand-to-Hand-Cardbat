using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Agent : PlayerCard
{
    public override void Setup()
    {
        logName = "an Agent";
        myColor = CardColor.Gold;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i<currPlayer.listOfHand.Count; i++)
        {
            PlayerCard nextCard = currPlayer.listOfHand[i];
            if (currPlayer.coins >= nextCard.myCost && nextCard.myCost <= 5)
                possibleCards.Add(nextCard);
        }

        yield return currPlayer.ChooseToPlay(possibleCards, "Agent");
    }

    public override IEnumerator PlayEffect(Player currPlayer)
    {
        yield return this.InitialCommand(currPlayer, this);
    }
}
