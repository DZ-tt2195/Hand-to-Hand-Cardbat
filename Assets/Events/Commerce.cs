using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Commerce : Event
{
    public override void Setup()
    {
        logName = "Commerce";
        thisTrigger = EventTrigger.TurnStart;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[5 - 1] = true;
        active[6 - 1] = true;
        active[7 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
        active[10 - 1] = true;
    }

    [PunRPC]
    public override IEnumerator UseEvent(Player currPlayer)
    {
        if (currPlayer.listOfPlay.Count >= 1)
        {
            currPlayer.MakeMeCollector($"Commerce", true);
            Manager.instance.instructions.text = $"Discard a card from play to play a card for free?";
            Collector x = currPlayer.newCollector;
            x.AddText("No", true);

            for (int i = 0; i< currPlayer.listOfPlay.Count; i++)
                currPlayer.listOfPlay[i].choicescript.EnableButton(currPlayer, true);

            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < currPlayer.listOfPlay.Count; i++)
                currPlayer.listOfPlay[i].choicescript.DisableButton();
            Destroy(x.gameObject);
            PlayerCard replacementCard;

            if (currPlayer.chosencard != null)
            {
                replacementCard = currPlayer.chosencard.GetComponent<PlayerCard>();
                yield return replacementCard.DiscardEffect(currPlayer, 2);
                List<PlayerCard> couldPlayForFree = new List<PlayerCard>();

                for (int i = 0; i<currPlayer.listOfHand.Count; i++)
                {
                    PlayerCard nextCard = currPlayer.listOfHand[i];
                    if (nextCard.myCost == replacementCard.myCost)
                        couldPlayForFree.Add(nextCard);
                }
                yield return currPlayer.ChooseToPlay(couldPlayForFree, "Commerce", true);
            }

        }
    }
}
