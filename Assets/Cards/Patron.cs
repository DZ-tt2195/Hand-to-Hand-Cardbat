using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Patron : PlayerCard
{
    public override void Setup()
    {
        logName = "a Patron";
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
            if (nextPlayer.coins <= 5)
                nextPlayer.TryToGain(3);

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }

        yield return new WaitForSeconds(0.5f);
        yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Patron");
    }
}
