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

            if (currPlayer.choice == "Yes")
            {
                currPlayer.cardsDiscardedThisTurn++;
                yield return currPlayer.FreePlayMe(this);
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
