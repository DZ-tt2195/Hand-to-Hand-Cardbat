using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Advisor : PlayerCard
{
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

        thisPlayer.MakeMeCollector($"Advisor", true);
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
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"draw 3.\"");
                this.pv.RPC("ExecuteChoice", thisPlayer.photonplayer, requestingPlayerPosition, "Draw 3");
                break;
            case "Gain $12":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"gain $12.\"");
                this.pv.RPC("ExecuteChoice", thisPlayer.photonplayer, requestingPlayerPosition, "Gain 12");
                break;
            case "Play 2":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{thisPlayer.name} chooses \"play 2 cards.\"");
                this.pv.RPC("ExecuteChoice", thisPlayer.photonplayer, requestingPlayerPosition, "Play 2");
                break;
        }
    }

    [PunRPC]
    public IEnumerator ExecuteChoice(int requestingPlayerPosition, string choice)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];

        switch (choice)
        {
            case "Draw 3":
                requestingPlayer.TryToDraw(3);
                break;
            case "Gain $12":
                requestingPlayer.TryToGain(12);
                break;
            case "Play 2":
                yield return requestingPlayer.ChooseToPlay(requestingPlayer.listOfHand, "Advisor");
                yield return requestingPlayer.ChooseToPlay(requestingPlayer.listOfHand, "Advisor");
                break;
        }

        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
