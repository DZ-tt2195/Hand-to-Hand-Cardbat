using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Vampire : PlayerCard
{
    public override void Setup()
    {
        logName = "a Vampire";
        myColor = CardColor.Gold;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToGain(5);
        int playertracker = currPlayer.playerposition;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            Manager.instance.instructions.text = $"Waiting for {nextPlayer.name}...";
            if (nextPlayer != currPlayer)
            {
                currPlayer.waiting = true;
                this.pv.RPC("Discard5", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition);

                while (currPlayer.waiting)
                    yield return null;
            }
            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    public IEnumerator Discard5(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);

        if (thisPlayer.listOfPlay.Count >= 5)
        {
            List<PlayerCard> possibleCards = new List<PlayerCard>();
            for (int i = 0; i<thisPlayer.listOfPlay.Count; i++)
            {
                if (thisPlayer.listOfPlay[i].myCost >= 5)
                    possibleCards.Add(thisPlayer.listOfPlay[i]);
            }

            PlayerCard toDiscard = null;
            Manager.instance.instructions.text = $"Discard a card from play";

            if (possibleCards.Count == 1)
            {
                toDiscard = possibleCards[0];
            }
            else if (possibleCards.Count == 0)
            {
                for (int i = 0; i < possibleCards.Count; i++)
                    possibleCards[i].choicescript.EnableButton(thisPlayer, true);

                yield return thisPlayer.WaitForDecision();

                for (int i = 0; i < possibleCards.Count; i++)
                    possibleCards[i].choicescript.DisableButton();

                toDiscard = thisPlayer.chosencard.GetComponent<PlayerCard>();
            }

            if (toDiscard != null)
                yield return toDiscard.DiscardEffect(thisPlayer, 2);
        }

        Manager.instance.playerOrderGame[requestingPlayerPosition].pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
