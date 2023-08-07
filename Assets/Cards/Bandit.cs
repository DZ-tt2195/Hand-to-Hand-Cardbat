using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bandit : PlayerCard
{
    public override void Setup()
    {
        logName = "a Bandit";
        myColor = CardColor.Blue;
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
                nextPlayer.pv.RPC("LoseCoin", RpcTarget.MasterClient, (currPlayer.lastUsedAction == Player.Actions.Recruit) ? 6 : 3);

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
