using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Performance : Event
{
    public override void Setup()
    {
        logName = "Performance";
        thisTrigger = EventTrigger.TurnEnd;
        active[1 - 1] = true;
        active[2 - 1] = true;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[5 - 1] = true;
        active[6 - 1] = true;
        active[7 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
        active[10 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        yield return null;
        if (currPlayer.cardsPlayedThisTurn.Count != 1)
        {
            currPlayer.pv.RPC("TakeCrown", RpcTarget.All, 1);
        }
    }
}
