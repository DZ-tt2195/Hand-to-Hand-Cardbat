using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Captain : PlayerCard
{
    public override void Setup()
    {
        logName = "a Captain";
        myColor = CardColor.Blue;
        myCost = 10;
        myCrowns = 3;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.MakeMeCollector($"Captain", true);
        Collector x = currPlayer.newCollector;

        for (int i = 2; i>0; i++)
        {
            Manager.instance.instructions.text = $"Discard a card to command it? ({i} more)";
            for (int j = 0; j<currPlayer.listOfHand.Count; j++)
            {
                currPlayer.listOfHand[j].choicescript.EnableButton(currPlayer, true);
            }

            yield return currPlayer.WaitForDecision();

            for (int j = 0; j < currPlayer.listOfHand.Count; j++)
                currPlayer.listOfHand[j].choicescript.DisableButton();

            PlayerCard commandMe = currPlayer.chosencard.GetComponent<PlayerCard>();
            if (commandMe == null)
            {
                break;
            }
            else
            {
                x.pv.RPC("AddCard", RpcTarget.All, commandMe.pv.ViewID, false);

                yield return commandMe.DiscardEffect(currPlayer, 1);
                yield return commandMe.InitialCommand(currPlayer, this);
            }
        }

        PhotonNetwork.Destroy(x.pv);
    }
}
