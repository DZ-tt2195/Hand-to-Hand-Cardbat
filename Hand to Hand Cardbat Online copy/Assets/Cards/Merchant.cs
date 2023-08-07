using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Merchant : PlayerCard
{
    public override void Setup()
    {
        logName = "a Merchant";
        myColor = CardColor.Red;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
        currPlayer.TryToGain(10);
        this.pv.RPC("MerchantDebt", RpcTarget.All, currPlayer.pv.ViewID);
        Log.instance.pv.RPC("AddText", RpcTarget.All, $"{currPlayer.name} will lose {currPlayer.merchantDebt} later.");
    }

    [PunRPC]
    public void MerchantDebt(int playerID)
    {
        Player thisPlayer = Manager.instance.playerOrderGame[playerID];
        thisPlayer.merchantDebt += 6;
    }
}
