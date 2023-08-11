using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Action : Card
{
    public virtual IEnumerator UseAction(Player currPlayer)
    {
        yield return null;
    }

    public IEnumerator ColorCommand(Player currPlayer, PlayerCard.CardColor thisColor)
    {
        this.logName = this.name;
        pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);
        yield return new WaitForSeconds(0.5f);

        List<PlayerCard> possibleCards = new List<PlayerCard>();
        List<PlayerCard> chosenCards = new List<PlayerCard>();

        for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
        {
            if (currPlayer.listOfPlay[i].myColor == thisColor && !currPlayer.listOfPlay[i].rotated)
            {
                possibleCards.Add(currPlayer.listOfPlay[i]);
            }
        }

        if (possibleCards.Count > 4)
        {
            for (int i = 0; i < possibleCards.Count; i++)
            {
                possibleCards[i].choicescript.EnableButton(currPlayer, true);
            }

            for (int j = 4; j > 0; j--)
            {
                Manager.instance.instructions.text = $"Choose {j} more cards.";
                yield return currPlayer.WaitForDecision();

                PlayerCard picked = currPlayer.chosencard.GetComponent<PlayerCard>();
                picked.choicescript.DisableButton();
                possibleCards.Remove(picked);
                chosenCards.Add(picked);
            }

            for (int i = 0; i < possibleCards.Count; i++)
            {
                possibleCards[i].choicescript.DisableButton();
            }
        }
        else
        {
            for (int i = 0; i < possibleCards.Count; i++)
                chosenCards.Add(possibleCards[i]);
        }

        if (chosenCards.Count >= 1)
        {
            currPlayer.MakeMeCollector($"{this.name}", false);
            Collector x = currPlayer.newCollector;

            for (int i = 0; i < chosenCards.Count; i++)
            {
                x.pv.RPC("AddCard", RpcTarget.All, chosenCards[i].pv.ViewID, true);
            }

            for (int i = 0; i < chosenCards.Count; i++)
            {
                pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);
                Manager.instance.instructions.text = $"Command a card.";
                x.EnableAll();
                yield return currPlayer.WaitForDecision();

                x.DisableAll();
                x.pv.RPC("DestroyButton", RpcTarget.All, currPlayer.chosenbutton.transform.GetSiblingIndex());
                yield return currPlayer.chosencard.GetComponent<PlayerCard>().InitialCommand(currPlayer, this);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void ActionEnd(Player currPlayer, PlayerCard.CardColor thisColor)
    {
        for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
        {
            if (thisColor == currPlayer.listOfPlay[i].myColor)
                currPlayer.listOfPlay[i].pv.RPC("RotateMe", RpcTarget.All);
            else
                currPlayer.listOfPlay[i].pv.RPC("UnRotateMe", RpcTarget.All);
        }
    }
}
