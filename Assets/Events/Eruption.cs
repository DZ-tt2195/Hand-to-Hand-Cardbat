using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Eruption : Event
{
    public override void Setup()
    {
        logName = "Eruption";
        thisTrigger = EventTrigger.TurnEnd;
        active[4 - 1] = true;
        active[8 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        List<PlayerCard> blueCards = new List<PlayerCard>();
        List<PlayerCard> goldCards = new List<PlayerCard>();
        List<PlayerCard> redCards = new List<PlayerCard>();

        for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
        {
            PlayerCard nextCard = currPlayer.listOfPlay[i];
            if (nextCard.myColor == PlayerCard.CardColor.Blue)
                blueCards.Add(nextCard);
            else if (nextCard.myColor == PlayerCard.CardColor.Gold)
                goldCards.Add(nextCard);
            else if (nextCard.myColor == PlayerCard.CardColor.Red)
                redCards.Add(nextCard);
        }

        if (blueCards.Count >= 1)
        {
            for (int i = 0; i<blueCards.Count; i++)
            {
                blueCards[i].choicescript.EnableButton(currPlayer, true);
            }
            Manager.instance.instructions.text = $"Discard a Blue card from play";
            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < blueCards.Count; i++)
                blueCards[i].choicescript.DisableButton();

            yield return currPlayer.chosencard.GetComponent<PlayerCard>().DiscardEffect(currPlayer, 2);
        }
        if (goldCards.Count >= 1)
        {
            for (int i = 0; i < goldCards.Count; i++)
            {
                blueCards[i].choicescript.EnableButton(currPlayer, true);
            }
            Manager.instance.instructions.text = $"Discard a Gold card from play";
            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < goldCards.Count; i++)
                goldCards[i].choicescript.DisableButton();

            yield return currPlayer.chosencard.GetComponent<PlayerCard>().DiscardEffect(currPlayer, 2);
        }
        if (redCards.Count >= 1)
        {
            for (int i = 0; i < redCards.Count; i++)
            {
                redCards[i].choicescript.EnableButton(currPlayer, true);
            }
            Manager.instance.instructions.text = $"Discard a Red card from play";
            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < redCards.Count; i++)
                redCards[i].choicescript.DisableButton();

            yield return currPlayer.chosencard.GetComponent<PlayerCard>().DiscardEffect(currPlayer, 2);
        }
    }
}
