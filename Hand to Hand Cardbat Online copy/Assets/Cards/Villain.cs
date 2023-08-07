using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Villain : PlayerCard
{
    public override void Setup()
    {
        logName = "a Villain";
        myColor = CardColor.Gold;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
        int playertracker = currPlayer.playerposition;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            if (nextPlayer != currPlayer)
                nextPlayer.pv.RPC("TakeCrown", RpcTarget.MasterClient, (nextPlayer.coins >= 10) ? 2 : 1);

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
