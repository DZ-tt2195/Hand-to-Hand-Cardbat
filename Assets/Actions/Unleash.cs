using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unleash : Action
{
    public override IEnumerator UseAction(Player currPlayer)
    {
        currPlayer.currentAction = Player.Actions.Unleash;

        int playertracker = currPlayer.playerposition;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            Manager.instance.instructions.text = $"Waiting for {nextPlayer.name}...";
            currPlayer.waiting = true;
            this.pv.RPC("PlayFalconer", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition);

            while (currPlayer.waiting)
                yield return null;

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }

        if (Manager.instance.EventActive("Vacation"))
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name} skips Unleash's instructions.");
        }
        else
        {
            currPlayer.pv.RPC("TakeCrown", RpcTarget.All, 5);
            currPlayer.TryToGain(10);
            yield return new WaitForSeconds(0.5f);
            yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Unleash");
        }

        ActionEnd(currPlayer, PlayerCard.CardColor.None);

        if (Manager.instance.EventActive("Meteor Shower"))
        {
            currPlayer.meteorShowerTurn++;
        }
    }

    [PunRPC]
    public IEnumerator PlayFalconer(int thisPlayerPosition, int requestingPlayerPosition)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);

        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i < thisPlayer.listOfHand.Count; i++)
        {
            if (thisPlayer.listOfHand[i].GetComponent<Falconer>() != null)
                possibleCards.Add(thisPlayer.listOfHand[i]);
        }

        for (int i = 0; i<possibleCards.Count; i++)
        {
            Manager.instance.instructions.text = $"Play a Falconer for free?";
            thisPlayer.MakeMeCollector($"Falconer", true);

            Collector x = thisPlayer.newCollector;
            x.AddText("Decline", true);
            possibleCards[i].choicescript.EnableButton(thisPlayer, true);

            yield return thisPlayer.WaitForDecision();

            Destroy(x.gameObject);
            possibleCards[i].choicescript.DisableButton();
            if (thisPlayer.chosencard.gameObject == possibleCards[i].gameObject)
            {
                thisPlayer.pv.RPC("FreePlayMe", RpcTarget.All, thisPlayer.chosencard.pv.ViewID);
            }
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
