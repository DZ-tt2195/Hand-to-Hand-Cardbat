using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Squire : PlayerCard
{
    public override void Setup()
    {
        logName = "a Squire";
        myColor = CardColor.Red;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToDraw(1);
        yield return new WaitForSeconds(0.5f);

        int playertracker = currPlayer.playerposition;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            Manager.instance.instructions.text = $"Waiting for {nextPlayer.name}...";
            currPlayer.waiting = true;
            this.pv.RPC("Play5", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition);

            while (currPlayer.waiting)
                yield return null;

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    public IEnumerator Play5(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, thisPlayer.name);

        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i < thisPlayer.listOfHand.Count; i++)
        {
            if (thisPlayer.listOfHand[i].myCost == 5 && thisPlayer.coins >= thisPlayer.listOfHand[i].myCost)
                possibleCards.Add(thisPlayer.listOfHand[i]);
        }
        yield return thisPlayer.ChooseToPlay(possibleCards, $"{requestingPlayer.name}'s Squire");
        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
