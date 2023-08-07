using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Inventor : PlayerCard
{
    public override void Setup()
    {
        logName = "an Inventor";
        myColor = CardColor.Red;
        myCost = 5;
        myCrowns = 2;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Inventor");

        currPlayer.MakeMeCollector($"Inventor", false);
        Manager.instance.instructions.text = $"Command a card with Inventor";
        Collector x = currPlayer.newCollector;
        for (int i = 0; i<currPlayer.cardsPlayedThisTurn.Count; i++)
        {
            x.pv.RPC("AddCard", RpcTarget.All, currPlayer.cardsPlayedThisTurn[i].pv.ViewID, true);
        }
        yield return currPlayer.WaitForDecision();
        PhotonNetwork.Destroy(x.pv);

        if (currPlayer.chosencard != null)
        {
            currPlayer.MakeMeCollector("Inventor", false);
            Collector y = currPlayer.newCollector;
            PlayerCard commandMe = currPlayer.chosencard.GetComponent<PlayerCard>();
            y.pv.RPC("AddCard", RpcTarget.All, commandMe.pv.ViewID, false);

            yield return commandMe.InitialCommand(currPlayer, this);
            PhotonNetwork.Destroy(y.pv);
        }
    }
}
