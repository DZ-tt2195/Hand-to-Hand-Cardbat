using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Party : Event
{
    public override void Setup()
    {
        logName = "Party";
        thisTrigger = EventTrigger.TurnStart;
        active[2 - 1] = true;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[5 - 1] = true;
        active[6 - 1] = true;
        active[7 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        currPlayer.MakeMeCollector($"Party", true);
        Manager.instance.instructions.text = $"Have all players draw 2 cards?";
        Collector x = currPlayer.newCollector;
        x.AddText("Yes", true);
        x.AddText("No", true);

        yield return currPlayer.WaitForDecision();

        Destroy(x.gameObject);
        if (currPlayer.choice == "Yes")
        {
            int playertracker = currPlayer.playerposition;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
                nextPlayer.pv.RPC("RequestDraw", RpcTarget.MasterClient, 2);
                playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
            }
        }
    }
}
