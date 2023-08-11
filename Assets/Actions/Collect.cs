using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Collect : Action
{
    public override IEnumerator UseAction(Player currPlayer)
    {
        currPlayer.currentAction = Player.Actions.Collect;

        if (!Manager.instance.EventActive("Vacation"))
        {
            currPlayer.TryToGain(5);
        }

        yield return ColorCommand(currPlayer, PlayerCard.CardColor.Gold);

        ActionEnd(currPlayer, PlayerCard.CardColor.Gold);
    }
}
