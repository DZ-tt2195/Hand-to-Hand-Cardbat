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
    }

    [PunRPC]
    public void MerchantDebt(int playerID)
    {
        Player thisPlayer = Manager.instance.playerOrderGame[playerID];
        thisPlayer.merchantDebt += 8;
        Log.instance.AddText($"{thisPlayer.name} will lose $8 later.");
    }
}
