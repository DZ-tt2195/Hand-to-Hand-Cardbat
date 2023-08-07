using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Protest : Event
{
    public override void Setup()
    {
        logName = "Protest";
        thisTrigger = EventTrigger.TurnStart;
        active[2 - 1] = true;
        active[3 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
        {
            PlayerCard nextCard = currPlayer.listOfPlay[i];
            if (nextCard.myCost == 0)
            {
                possibleCards.Add(nextCard);
                nextCard.choicescript.EnableButton(currPlayer, true);
            }
        }

        Manager.instance.instructions.text = $"Command one of your $0 cards.";

        if (possibleCards.Count > 0)
        {
            yield return currPlayer.WaitForDecision();
            for (int i = 0; i < possibleCards.Count; i++)
                possibleCards[i].choicescript.DisableButton();

            currPlayer.MakeMeCollector($"{this.name}", false);
            Collector x = currPlayer.newCollector;
            PlayerCard commandMe = currPlayer.chosencard.GetComponent<PlayerCard>();
            x.pv.RPC("AddCard", RpcTarget.All, commandMe.pv.ViewID, true);

            yield return commandMe.InitialCommand(currPlayer, this);
            PhotonNetwork.Destroy(x.pv);

        }

    }
}
