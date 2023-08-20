using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Leprechaun : PlayerCard
{
    public override void Setup()
    {
        logName = "a Leprechaun";
        myColor = CardColor.Red;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToGain(10);
        currPlayer.pv.RPC("TakeCrown", RpcTarget.All, (Manager.instance.turnNumber >= 6 - 1) ? 2 : 0);
        yield return null;
    }
}
