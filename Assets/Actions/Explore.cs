using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explore : Action
{
    public override IEnumerator UseAction(Player currPlayer)
    {
        currPlayer.currentAction = Player.Actions.Explore;

        if (Manager.instance.EventActive("Vacation"))
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name} skips Explore's instructions.");
        }
        else
        {
            currPlayer.TryToDraw(2);
        }

        yield return ColorCommand(currPlayer, PlayerCard.CardColor.Blue);

        ActionEnd(currPlayer, PlayerCard.CardColor.Blue);
    }
}
