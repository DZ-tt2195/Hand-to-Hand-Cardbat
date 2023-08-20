using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Horse : PlayerCard
{
    public override void Setup()
    {
        logName = "a Horse";
        myColor = CardColor.Blue;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        int playertracker = currPlayer.playerposition;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            nextPlayer.pv.RPC("RequestDraw", RpcTarget.MasterClient, (nextPlayer == currPlayer) ? 2 : 1);
            yield return new WaitForSeconds(0.5f);
            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
