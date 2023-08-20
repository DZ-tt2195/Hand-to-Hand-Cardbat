using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Golem : PlayerCard
{
    public bool GetCard;
    public bool GetCoin;
    public bool GetPlay;

    public override void Setup()
    {
        logName = "a Golem";
        myColor = CardColor.Gold;
        myCost = 5;
        myCrowns = 2;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        GetCard = true;
        GetCoin = true;
        GetPlay = true;

        Player prevPlayer = currPlayer.GetPreviousPlayer();
        this.pv.RPC("MakeChoice", prevPlayer.photonplayer, prevPlayer.playerposition, currPlayer.playerposition);
        currPlayer.waiting = true;

        while (currPlayer.waiting)
            yield return null;

        if (GetCard)
        {
            currPlayer.TryToDraw(1);
            yield return new WaitForSeconds(0.25f);
        }
        if (GetCoin)
        {
            currPlayer.TryToGain(5);
            yield return new WaitForSeconds(0.25f);
        }
        if (GetPlay)
        {
            yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Golem");
        }

    }

    [PunRPC]
    public IEnumerator MakeChoice(int thisPlayerPosition, int requestingPlayerPosition)
    {
        yield return null;
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);

        thisPlayer.MakeMeCollector($"Hunter", true);
        Manager.instance.instructions.text = $"Choose one for {requestingPlayer.name}'s Golem to ignore.";
        Collector x = thisPlayer.newCollector;

        x.AddText("Draw 1", true);
        x.AddText("Gain $5", true);
        x.AddText("Play 1", true);

        yield return thisPlayer.WaitForDecision();
        Destroy(x.gameObject);
        string ignoreThis = thisPlayer.choice;

        switch (ignoreThis)
        {
            case "Draw 1":
                GetCard = false;
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} blocks \"draw 1.\"");
                break;
            case "Gain $5":
                GetCoin = false;
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} blocks \"gain $5.\"");
                break;
            case "Play 1":
                GetPlay = false;
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} blocks \"play a card.\"");
                break;
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
