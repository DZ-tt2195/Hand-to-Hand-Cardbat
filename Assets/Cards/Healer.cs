using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Healer : PlayerCard
{
    public override void Setup()
    {
        logName = "a Healer";
        myColor = CardColor.Gold;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
        currPlayer.TryToGain(5);
        currPlayer.pv.RPC("LoseCrown", RpcTarget.All,
        (currPlayer.GetPreviousPlayer().lastUsedAction == Player.Actions.Collect)
        ? 2 : 0);
    }

}
