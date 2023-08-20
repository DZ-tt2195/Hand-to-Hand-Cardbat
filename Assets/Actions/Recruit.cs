using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Recruit : Action
{
    public override IEnumerator UseAction(Player currPlayer)
    {
        currPlayer.currentAction = Player.Actions.Recruit;

        if (Manager.instance.EventActive("Vacation"))
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name} skips Recruit's instructions.");
        }
        else
        { 
            yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Recruit");
        }

        if (Manager.instance.turnNumber > 1 - 1)
            yield return ColorCommand(currPlayer, PlayerCard.CardColor.Red);

        ActionEnd(currPlayer, PlayerCard.CardColor.Red);
    }
}
