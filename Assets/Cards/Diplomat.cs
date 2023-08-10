using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Diplomat : PlayerCard
{
    public override void Setup()
    {
        logName = "a Diplomat";
        myColor = CardColor.Gold;
        myCost = 5;
        myCrowns = 2;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToDraw(1);

        yield return new WaitForSeconds(0.5f);
        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i < currPlayer.listOfHand.Count; i++)
        {
            if (currPlayer.listOfHand[i].myCost <= 5)
                possibleCards.Add(currPlayer.listOfHand[i]);
        }

        if (possibleCards.Count >= 1)
        {
            currPlayer.MakeMeCollector($"Diplomat", true);

            for (int i = 0; i < possibleCards.Count; i++)
                possibleCards[i].choicescript.EnableButton(currPlayer, true);

            Manager.instance.instructions.text = $"Command a card with Diplomat?";
            Collector x = currPlayer.newCollector;
            x.AddText("Decline", true);

            yield return currPlayer.WaitForDecision();

            for (int i = 0; i < possibleCards.Count; i++)
                possibleCards[i].choicescript.DisableButton();
            Destroy(x.gameObject);

            if (currPlayer.chosencard != null)
            {
                currPlayer.MakeMeCollector("Diplomat", false);
                Collector y = currPlayer.newCollector;

                PlayerCard commandMe = currPlayer.chosencard.GetComponent<PlayerCard>();
                y.pv.RPC("AddCard", RpcTarget.All, commandMe.pv.ViewID, false);

                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{currPlayer.name} reveals {commandMe.logName}.");
                yield return commandMe.InitialCommand(currPlayer, this);
                PhotonNetwork.Destroy(y.pv);
            }
        }
    }
}
