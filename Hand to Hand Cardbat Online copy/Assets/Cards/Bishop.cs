using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bishop : PlayerCard
{
    public override void Setup()
    {
        logName = "a Bishop";
        myColor = CardColor.Blue;
        myCost = 5;
        myCrowns = 2;
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
            this.pv.RPC("BishopEffect", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition);

            while (currPlayer.waiting)
                yield return null;

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    public IEnumerator BishopEffect(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];

        List<PlayerCard> toDiscard = new List<PlayerCard>() ;

        if (thisPlayer.listOfHand.Count >= 3)
        {
            thisPlayer.MakeMeCollector("Bishop", true);
            Collector x = thisPlayer.newCollector;
            x.AddText("Decline", true);

            for (int i = 3; i > 0; i--)
            {
                Manager.instance.instructions.text = $"Discard to Bishop? ({i} more)";

                for (int j = 0; j < toDiscard.Count; j++)
                {
                    toDiscard[j].choicescript.EnableButton(thisPlayer, true);
                }

                yield return thisPlayer.WaitForDecision();

                for (int j = 0; j < toDiscard.Count; j++)
                    toDiscard[j].choicescript.DisableButton();

                PlayerCard nextCard = thisPlayer.chosencard.GetComponent<PlayerCard>();
                if (nextCard != null)
                    toDiscard.Add(nextCard);
                else
                    break;
            }

            for (int i = 0; i < toDiscard.Count; i++)
            {
                yield return toDiscard[i].DiscardEffect(thisPlayer, 1);
            }

            Destroy(x.gameObject);
        }

        if (toDiscard.Count < 3)
        {
            thisPlayer.TryToDraw(2);
            thisPlayer.pv.RPC("TakeCrown", RpcTarget.All, 2);
        }
        else if (toDiscard.Count == 3)
        {
            thisPlayer.pv.RPC("LoseCrown", RpcTarget.All, 3);
        }

        Manager.instance.playerOrderGame[requestingPlayerPosition].pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
