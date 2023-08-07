using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cleaning : Event
{
    public override void Setup()
    {
        logName = "Cleaning";
        thisTrigger = EventTrigger.TurnEnd;
        active[2 - 1] = true;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[5 - 1] = true;
        active[6 - 1] = true;
        active[7 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
    }

    public override IEnumerator UseEvent(Player currPlayer)
    {
        yield return null;
        int playertracker = currPlayer.playerposition;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            nextPlayer.TryToGain(nextPlayer.cardsDiscardedThisTurn);
            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }
}
