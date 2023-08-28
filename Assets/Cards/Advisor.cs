using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Advisor : PlayerCard
{
    public bool GetCard;
    public bool GetCoin;
    public bool GetPlay;

    public override void Setup()
    {
        logName = "an Advisor";
        myColor = CardColor.Blue;
        myCost = 10;
        myCrowns = 3;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        GetCard = false;
        GetCoin = false;
        GetPlay = false;

        Player prevPlayer = currPlayer.GetPreviousPlayer();
        this.pv.RPC("MakeChoice", prevPlayer.photonplayer, prevPlayer.playerposition, currPlayer.playerposition);
        currPlayer.waiting = true;

        while (currPlayer.waiting)
            yield return null;

        if (GetCard)
        {
            currPlayer.TryToDraw(3);
        }
        if (GetCoin)
        {
            currPlayer.TryToGain(12);
        }
        if (GetPlay)
        {
            yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Golem");
            yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Golem");
        }
    }

    [PunRPC]
    public IEnumerator MakeChoice(int thisPlayerPosition, int requestingPlayerPosition)
    {
        yield return null;
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, thisPlayer.name);

        thisPlayer.MakeMeCollector($"Hunter", true);
        Manager.instance.instructions.text = $"Choose one for {requestingPlayer.name}'s Advisor to do.";
        Collector x = thisPlayer.newCollector;

        x.AddText("Draw 3", true);
        x.AddText("Gain $12", true);
        x.AddText("Play 2", true);

        yield return thisPlayer.WaitForDecision();
        Destroy(x.gameObject);
        string ignoreThis = thisPlayer.choice;

        switch (ignoreThis)
        {
            case "Draw 3":
                GetCard = true;
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"draw 3.\"");
                break;
            case "Gain $12":
                GetCoin = true;
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"gain $12.\"");
                break;
            case "Play 2":
                GetPlay = true;
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"play 2 cards.\"");
                break;
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
