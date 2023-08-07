using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tournament : Event
{
    public override void Setup()
    {
        logName = "Tournament";
        thisTrigger = EventTrigger.TurnEnd;
        active[3 - 1] = true;
        active[5 - 1] = true;
        active[7 - 1] = true;
        active[9 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        bool mostCards = true;
        int playertracker = currPlayer.playerposition;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            if (nextPlayer != currPlayer && nextPlayer.listOfHand.Count >= currPlayer.listOfHand.Count)
                mostCards = false;

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }

        if (mostCards)
            currPlayer.TryToGain(4);
        yield return null;
    }
}
