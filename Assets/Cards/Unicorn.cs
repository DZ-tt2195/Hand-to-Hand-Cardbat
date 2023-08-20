using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unicorn : PlayerCard
{
    public override void Setup()
    {
        logName = "a Unicorn";
        myColor = CardColor.Red;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.pv.RPC("LoseCrown", RpcTarget.All, 1);
        yield return null;
    }

    public override IEnumerator PlayEffect(Player currPlayer)
    {
        currPlayer.TryToGain(10);
        yield return null;
    }
}
