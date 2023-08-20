using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sphinx : PlayerCard
{
    public override void Setup()
    {
        logName = "a Sphinx";
        myColor = CardColor.Gold;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        if (currPlayer.thisTurn == Player.TypeOfTurn.Normal && currPlayer.coins >= 8)
        {
            currPlayer.MakeMeCollector($"Sphinx", true);
            Manager.instance.instructions.text = $"Pay $8 for an extra turn?";

            Collector x = currPlayer.newCollector;
            x.AddText("Yes", true);
            x.AddText("No", true);

            yield return currPlayer.WaitForDecision();

            Destroy(x.gameObject);
            if (currPlayer.choice == "Yes")
                currPlayer.sphinxTurn++;

        }
    }
}
