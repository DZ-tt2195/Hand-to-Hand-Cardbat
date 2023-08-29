using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spy : PlayerCard
{
    public override void Setup()
    {
        logName = "a Spy";
        myColor = CardColor.Blue;
        myCost = 0;
        myCrowns = 1;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.MakeMeCollector("Spy", false);
        Collector x = currPlayer.newCollector;

        List<PlayerCard> otherPlayersCards = currPlayer.GetPreviousPlayer().listOfHand;
        bool whiffed = true;

        for (int i = 0; i < otherPlayersCards.Count; i++)
        {
            PlayerCard nextCard = otherPlayersCards[i];
            x.pv.RPC("AddCard", RpcTarget.All, nextCard.pv.ViewID, (nextCard.myCost <= 5));
            if (nextCard.myCost <= 5)
                whiffed = false;
        }
        if (whiffed)
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"Spy has nothing to command.");
        }
        else
        {
            yield return currPlayer.WaitForDecision();

            PlayerCard commandMe = currPlayer.chosencard.GetComponent<PlayerCard>();
            x.pv.RPC("DestroyOtherButtons", RpcTarget.All, commandMe.pv.ViewID);

            yield return commandMe.InitialCommand(currPlayer, this);
        }
        PhotonNetwork.Destroy(x.pv);
    }
}
