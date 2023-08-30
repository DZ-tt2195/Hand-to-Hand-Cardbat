using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Expedition : Event
{
    public override void Setup()
    {
        logName = "Expedition";
        thisTrigger = EventTrigger.TurnEnd;
        active[1 - 1] = true;
        active[2 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        if (currPlayer.currentAction == Player.Actions.Explore)
        {
            for (int i = 0; i < 2; i++)
            {
                List<PlayerCard> cardsOf0 = new List<PlayerCard>();
                for (int j = 0; j < currPlayer.listOfHand.Count; j++)
                {
                    if (currPlayer.listOfHand[j].myCost == 0 && currPlayer.coins >= currPlayer.listOfHand[j].myCost)
                        cardsOf0.Add(currPlayer.listOfHand[j]);
                }

                yield return currPlayer.ChooseToPlay(cardsOf0, "Expedition");
            }
        }
    }
}
