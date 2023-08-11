using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Raider : PlayerCard
{
    public override void Setup()
    {
        logName = "a Raider";
        myColor = CardColor.Red;
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
            Manager.instance.instructions.text = $"Waiting for {nextPlayer.name}...";
            currPlayer.waiting = true;
            if (nextPlayer != currPlayer)
                this.pv.RPC("DiscardTo2", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition);

            while (currPlayer.waiting)
                yield return null;

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    public IEnumerator DiscardTo2(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);

        if (thisPlayer.listOfHand.Count > 2)
        {
            List<PlayerCard> toDiscard = thisPlayer.listOfHand;

            for (int i = 2; i>0; i--)
            { 
                Manager.instance.instructions.text = $"Choose {i} cards to keep in hand.";

                for (int j = 0; j < toDiscard.Count; j++)
                {
                    toDiscard[j].choicescript.EnableButton(thisPlayer, true);
                }

                yield return thisPlayer.WaitForDecision();

                for (int j = 0; j < toDiscard.Count; j++)
                    toDiscard[j].choicescript.DisableButton();

                toDiscard.Remove(thisPlayer.chosencard.GetComponent<PlayerCard>());
            }

            for (int i = 0; i<toDiscard.Count; i++)
            {
                yield return toDiscard[i].DiscardEffect(thisPlayer, 1);
            }
        }

        Manager.instance.playerOrderGame[requestingPlayerPosition].pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
