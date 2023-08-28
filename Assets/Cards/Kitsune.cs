using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Kitsune : PlayerCard
{
    public override void Setup()
    {
        logName = "a Kitsune";
        myColor = CardColor.Blue;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Kitsune");

        currPlayer.MakeMeCollector($"Commerce", true);
        Manager.instance.instructions.text = $"Choose a card you've in play to exchange?";
        Collector x = currPlayer.newCollector;
        x.AddText("Don't Exchange", true);

        for (int i = 0; i < currPlayer.listOfPlay.Count; i++)
            currPlayer.listOfPlay[i].choicescript.EnableButton(currPlayer, true);

        yield return currPlayer.WaitForDecision();
        Destroy(x.gameObject);

        for (int i = 0; i < currPlayer.listOfPlay.Count; i++)
            currPlayer.listOfPlay[i].choicescript.DisableButton();

        if (currPlayer.chosencard != null)
        {
            PlayerCard myExchange = currPlayer.chosencard.GetComponent<PlayerCard>();
            List<PlayerCard> couldExchangeFor = new List<PlayerCard>();

            for (int i = 0; i < currPlayer.GetPreviousPlayer().listOfHand.Count; i++)
            {
                PlayerCard nextCard = currPlayer.GetPreviousPlayer().listOfHand[i];
                if (nextCard.myCost == myExchange.myCost)
                    couldExchangeFor.Add(nextCard);
            }

            if (couldExchangeFor.Count >= 1)
            {
                for (int i = 0; i < couldExchangeFor.Count; i++)
                    couldExchangeFor[i].choicescript.EnableButton(currPlayer, true);

                yield return currPlayer.WaitForDecision();

                for (int i = 0; i < currPlayer.GetPreviousPlayer().listOfPlay.Count; i++)
                    currPlayer.GetPreviousPlayer().listOfPlay[i].choicescript.DisableButton();

                PlayerCard theirExchange = currPlayer.chosencard.GetComponent<PlayerCard>();
                this.pv.RPC("ExchangeCards", RpcTarget.All, currPlayer.playerposition, currPlayer.GetPreviousPlayer().playerposition, myExchange.pv.ViewID, theirExchange.pv.ViewID);
            }
        }
    }

    [PunRPC]
    public void ExchangeCards(int currPlayerPosition, int prevPlayerPosition, int currPlayerCard, int prevPlayerCard)
    {
        Player currPlayer = Manager.instance.playerOrderGame[currPlayerPosition];
        Player prevPlayer = Manager.instance.playerOrderGame[prevPlayerPosition];

        PlayerCard currExchange = PhotonView.Find(currPlayerCard).GetComponent<PlayerCard>();
        PlayerCard prevExchange = PhotonView.Find(prevPlayerCard).GetComponent<PlayerCard>();

        currPlayer.listOfPlay.Remove(currExchange);
        prevPlayer.listOfPlay.Remove(prevExchange);

        currPlayer.AddCardByColor(prevExchange, 0, currPlayer.listOfPlay.Count, false);
        prevPlayer.AddCardByColor(currExchange, 0, prevPlayer.listOfPlay.Count, false);

        Log.instance.AddText($"{currExchange.logName} and {prevExchange.logName} are exchanged.");
        Log.instance.AddText($"{currPlayer.name} receives {prevExchange.logName}.");
        Log.instance.AddText($"{prevPlayer.name} receives {currExchange.logName}.");
    }
}