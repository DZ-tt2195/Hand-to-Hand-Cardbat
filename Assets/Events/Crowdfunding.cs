using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crowdfunding : Event
{
    public override void Setup()
    {
        logName = "Crowdfunding";
        thisTrigger = EventTrigger.TurnEnd;
        active[1 - 1] = true;
        active[2 - 1] = true;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[5 - 1] = true;
        active[6 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        if (currPlayer.coins >= 5 && currPlayer.negCrowns >= 1)
        {
            currPlayer.MakeMeCollector($"Crowdfunding", true);
            Manager.instance.instructions.text = $"Pay $5 to remove -2 Crowns?";

            Collector x = currPlayer.newCollector;
            x.AddText("Yes", true);
            x.AddText("No", true);

            yield return currPlayer.WaitForDecision();

            Destroy(x.gameObject);
            if (currPlayer.choice == "Yes")
            {
                currPlayer.pv.RPC("LoseCoin", RpcTarget.All, 5);
                currPlayer.pv.RPC("LoseCrown", RpcTarget.All, 2);
            }
        }
    }
}
