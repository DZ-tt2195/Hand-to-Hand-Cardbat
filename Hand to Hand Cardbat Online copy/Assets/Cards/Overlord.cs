using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Overlord : PlayerCard
{
    public override void Setup()
    {
        logName = "a Overlord";
        myColor = CardColor.Red;
        myCost = 10;
        myCrowns = 3;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        List<PlayerCard> blueCards = new List<PlayerCard>();
        List<PlayerCard> goldCards = new List<PlayerCard>();

        for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
        {
            if (currPlayer.listOfPlay[i].myColor == CardColor.Blue)
                blueCards.Add(currPlayer.listOfPlay[i]);
            else if (currPlayer.listOfPlay[i].myColor == CardColor.Gold)
                goldCards.Add(currPlayer.listOfPlay[i]);
        }

        currPlayer.MakeMeCollector($"Overlord", false);
        Manager.instance.instructions.text = $"Command a Blue card with Overlord";
        Collector x = currPlayer.newCollector;

        PlayerCard commandBlue = null;
        if (blueCards.Count == 1)
        {
            commandBlue = (blueCards[0]);
        }
        else if (blueCards.Count >= 2)
        {
            Manager.instance.instructions.text = $"Choose a Blue card.";
            for (int i = 0; i < blueCards.Count; i++)
                blueCards[i].choicescript.EnableButton(currPlayer, true);
            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < blueCards.Count; i++)
                blueCards[i].choicescript.DisableButton();
            commandBlue = currPlayer.chosencard.GetComponent<PlayerCard>();
        }
        if (commandBlue != null)
        {
            x.pv.RPC("AddCard", RpcTarget.All, commandBlue.pv.ViewID, false);
            yield return commandBlue.InitialCommand(currPlayer, this);
        }

        PlayerCard commandGold = null;
        if (goldCards.Count == 1)
        {
            commandGold = (blueCards[0]);
        }
        else if (blueCards.Count >= 2)
        {
            Manager.instance.instructions.text = $"Choose a Gold card.";
            for (int i = 0; i < blueCards.Count; i++)
                blueCards[i].choicescript.EnableButton(currPlayer, true);
            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < blueCards.Count; i++)
                blueCards[i].choicescript.DisableButton();
            commandGold = currPlayer.chosencard.GetComponent<PlayerCard>();
        }
        if (commandGold != null)
        {
            x.pv.RPC("AddCard", RpcTarget.All, commandGold.pv.ViewID, false);
            yield return commandBlue.InitialCommand(currPlayer, this);
        }

        PhotonNetwork.Destroy(x.pv);
    }
}
