using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Golem : PlayerCard
{
    public override void Setup()
    {
        logName = "a Golem";
        myColor = CardColor.Gold;
        myCost = 5;
        myCrowns = 32;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        Player prevPlayer = currPlayer.GetPreviousPlayer();
        this.pv.RPC("MakeChoice", prevPlayer.photonplayer, prevPlayer.playerposition, currPlayer.playerposition);
        currPlayer.waiting = true;

        while (currPlayer.waiting)
            yield return null;
    }

    [PunRPC]
    public IEnumerator MakeChoice(int thisPlayerPosition, int requestingPlayerPosition)
    {
        yield return null;
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, thisPlayer.name);

        thisPlayer.MakeMeCollector($"Golem", true);
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
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"draw 1.\"");
                this.pv.RPC("ExecuteChoice", thisPlayer.photonplayer, requestingPlayerPosition, "Draw 1");
                break;
            case "Gain $5":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"gain $5.\"");
                this.pv.RPC("ExecuteChoice", thisPlayer.photonplayer, requestingPlayerPosition, "Gain 5");
                break;
            case "Play 1":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"play 1 card.\"");
                this.pv.RPC("ExecuteChoice", thisPlayer.photonplayer, requestingPlayerPosition, "Play 1");
                break;
        }
    }

    [PunRPC]
    public IEnumerator ExecuteChoice(int requestingPlayerPosition, string choice)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];

        switch (choice)
        {
            case "Draw 1":
                requestingPlayer.TryToGain(5);
                yield return requestingPlayer.ChooseToPlay(requestingPlayer.listOfHand, "Golem");
                break;
            case "Gain $5":
                requestingPlayer.TryToDraw(1);
                yield return requestingPlayer.ChooseToPlay(requestingPlayer.listOfHand, "Golem");
                break;
            case "Play 1":
                requestingPlayer.TryToDraw(1);
                requestingPlayer.TryToGain(5);
                break;
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
