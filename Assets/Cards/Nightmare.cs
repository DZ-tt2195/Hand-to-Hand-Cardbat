using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Nightmare : PlayerCard
{
    public override void Setup()
    {
        logName = "a Nightmare";
        myColor = CardColor.Red;
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
            if (nextPlayer != currPlayer)
            {
                currPlayer.waiting = true;
                this.pv.RPC("RotateTo2", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition);

                while (currPlayer.waiting)
                    yield return null;
            }
            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    public IEnumerator RotateTo2(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);

        if (thisPlayer.listOfPlay.Count > 2)
        {
            List<PlayerCard> toRotate = thisPlayer.listOfPlay;
            List<PlayerCard> keptSafe = new List<PlayerCard>() ;

            for (int i = 2; i > 0; i--)
            {
                Manager.instance.instructions.text = $"Choose {i} cards to keep un-rotated.";

                for (int j = 0; j < toRotate.Count; j++)
                {
                    toRotate[j].choicescript.EnableButton(thisPlayer, true);
                }

                yield return thisPlayer.WaitForDecision();

                for (int j = 0; j < toRotate.Count; j++)
                    toRotate[j].choicescript.DisableButton();

                PlayerCard nextCard = thisPlayer.chosencard.GetComponent<PlayerCard>();
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} keeps {nextCard.logName} un-rotated.");
                keptSafe.Add(nextCard);
                toRotate.Remove(nextCard);
            }

            for (int i = 0; i < toRotate.Count; i++)
            {
                toRotate[i].pv.RPC("RotateMe", RpcTarget.All);
            }
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
