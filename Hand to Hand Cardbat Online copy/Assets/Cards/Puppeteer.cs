using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Puppeteer : PlayerCard
{
    public override void Setup()
    {
        logName = "a Puppeteer";
        myColor = CardColor.Gold;
        myCost = 10;
        myCrowns = 3;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        List<PlayerCard> cost0 = new List<PlayerCard>();
        List<PlayerCard> cost5 = new List<PlayerCard>();
        List<PlayerCard> cost10 = new List<PlayerCard>();

        if (currPlayer.GetPreviousPlayer().listOfPlay.Count >= 1)
        {
            for (int i = 0; i < currPlayer.GetPreviousPlayer().listOfPlay.Count; i++)
            {
                PlayerCard nextCard = currPlayer.GetPreviousPlayer().listOfPlay[i];
                nextCard.choicescript.EnableButton(currPlayer, true);
                switch (nextCard.myCost)
                {
                    case 0:
                        cost0.Add(nextCard);
                        break;
                    case 5:
                        cost5.Add(nextCard);
                        break;
                    case 10:
                        cost10.Add(nextCard);
                        break;
                }
            }

            Manager.instance.instructions.text = $"Command one of {currPlayer.GetPreviousPlayer().name}'s cards";
            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < currPlayer.GetPreviousPlayer().listOfPlay.Count; i++)
            {
                currPlayer.GetPreviousPlayer().listOfPlay[i].choicescript.DisableButton();
            }

            PlayerCard firstCommand = currPlayer.chosencard.GetComponent<PlayerCard>();
            currPlayer.MakeMeCollector($"Overlord", false);
            Collector x = currPlayer.newCollector;
            x.pv.RPC("AddCard", RpcTarget.All, firstCommand.pv.ViewID, false);
            yield return firstCommand.InitialCommand(currPlayer, this);

            int otherChoices = 0;

            for (int i = 0; i < cost0.Count; i++)
            {
                if (firstCommand.myCost != 0)
                {
                    cost0[i].choicescript.EnableButton(currPlayer, true);
                    otherChoices++;
                }
            }
            for (int i = 0; i < cost5.Count; i++)
            {
                if (firstCommand.myCost != 5)
                {
                    cost5[i].choicescript.EnableButton(currPlayer, true);
                    otherChoices++;
                }
            }
            for (int i = 0; i < cost10.Count; i++)
            {
                if (firstCommand.myCost != 10)
                {
                    cost10[i].choicescript.EnableButton(currPlayer, true);
                    otherChoices++;
                }
            }

            if (otherChoices >= 1)
            {
                Manager.instance.instructions.text = $"Command another card with a different cost";
                yield return currPlayer.WaitForDecision();

                for (int i = 0; i < currPlayer.GetPreviousPlayer().listOfPlay.Count; i++)
                {
                    currPlayer.GetPreviousPlayer().listOfPlay[i].choicescript.DisableButton();
                }

                PlayerCard secondCommand = currPlayer.chosencard.GetComponent<PlayerCard>();
                x.pv.RPC("AddCard", RpcTarget.All, firstCommand.pv.ViewID, false);
                yield return secondCommand.InitialCommand(currPlayer, this);
            }

            PhotonNetwork.Destroy(x.pv);
        }
    }
}
