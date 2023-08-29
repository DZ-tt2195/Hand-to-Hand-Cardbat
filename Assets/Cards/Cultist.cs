using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cultist : PlayerCard
{
    public override void Setup()
    {
        logName = "a Cultist";
        myColor = CardColor.Red;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToDraw(2);
        yield return null;
    }

    public override IEnumerator DiscardEffect(Player currPlayer, int code)
    {
        if (code == 1)
        {
            Manager.instance.instructions.text = $"Play a Cultist for free?";
            currPlayer.MakeMeCollector("Cultist", true);
            Collector y = currPlayer.newCollector;

            y.AddText("Yes", true);
            y.AddText("No", true);
            yield return currPlayer.WaitForDecision();
            Destroy(y.gameObject);

            if (currPlayer.choice == "Yes")
            {
                Log.instance.AddText($"{this.name} discards {logName} from their hand.");
                currPlayer.pv.RPC("FreePlayMe", RpcTarget.All, this.pv.ViewID);

                if (currPlayer.pv.AmOwner && Manager.instance.EventActive("Cleaning"))
                    currPlayer.TryToGain(1);
            }
            else
            {
                yield return base.DiscardEffect(currPlayer, code);
            }
        }
        else
        {
            yield return base.DiscardEffect(currPlayer, code);
        }
    }
}
