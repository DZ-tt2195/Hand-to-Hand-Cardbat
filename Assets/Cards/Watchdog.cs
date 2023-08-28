using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Watchdog : PlayerCard
{
    public override void Setup()
    {
        logName = "a Watchdog";
        myColor = CardColor.Red;
        myCost = 0;
        myCrowns = 1;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.MakeMeCollector($"Watchdog", false);
        Manager.instance.instructions.text = $"Return a card to your hand to command it?";
        Collector x = currPlayer.newCollector;
        x.AddText("Decline", true);

        for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
            currPlayer.listOfPlay[i].choicescript.EnableButton(currPlayer, true);

        yield return currPlayer.WaitForDecision();
        Destroy(x.gameObject);

        for (int i = 0; i < currPlayer.listOfPlay.Count; i++)
            currPlayer.listOfPlay[i].choicescript.DisableButton();

        if (currPlayer.chosencard != null)
        {
            PlayerCard returnMe = currPlayer.chosencard.GetComponent<PlayerCard>();
            int[] cardsToReturn = new int[1];
            cardsToReturn[0] = returnMe.pv.ViewID;
            currPlayer.pv.RPC("SendDraw", RpcTarget.All, cardsToReturn, true);
            yield return new WaitForSeconds(0.25f);
            yield return returnMe.InitialCommand(currPlayer, this);
        }
    }
}