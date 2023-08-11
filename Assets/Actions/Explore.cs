using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explore : Action
{
    public override IEnumerator UseAction(Player currPlayer)
    {
        currPlayer.currentAction = Player.Actions.Explore;

        if (Manager.instance.EventActive("Expedition"))
        {
            List<PlayerCard> cardsOf0 = new List<PlayerCard>();
            for (int i = 0; i < currPlayer.listOfHand.Count; i++)
            {
                if (currPlayer.listOfHand[i].myCost == 0 && currPlayer.coins >= currPlayer.listOfHand[i].myCost)
                    cardsOf0.Add(currPlayer.listOfHand[i]);
            }
            for (int i = 0; i < 2; i++)
            {
                yield return currPlayer.ChooseToPlay(cardsOf0, "Expedition");
            }
        }

        if (!Manager.instance.EventActive("Vacation"))
        {
            currPlayer.TryToDraw(2);
        }

        yield return ColorCommand(currPlayer, PlayerCard.CardColor.Blue);

        ActionEnd(currPlayer, PlayerCard.CardColor.Blue);
    }
}
