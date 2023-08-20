using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Magpie : PlayerCard
{
    public override void Setup()
    {
        logName = "a Magpie";
        myColor = CardColor.Gold;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        for (int i = 0; i < 4; i++)
        {
            if (currPlayer.coins >= 2)
            {
                currPlayer.MakeMeCollector($"Magpie", true);
                Manager.instance.instructions.text = $"Pay $2 to draw a card?";
                Collector x = currPlayer.newCollector;

                x.AddText("Yes", true);
                x.AddText("No", true);

                yield return currPlayer.WaitForDecision();
                Destroy(x.gameObject);
                if (currPlayer.choice == "Yes")
                {
                    currPlayer.pv.RPC("LoseCoin", RpcTarget.All, 2);
                    currPlayer.pv.RPC("RequestDraw", RpcTarget.MasterClient, 1);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
}
