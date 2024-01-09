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
        myCrowns = 2;
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
    public IEnumerator MakeChoice(int prevPlayerPosition, int currPlayerPosition)
    {
        yield return null;
        Player currPlayer = Manager.instance.playerOrderGame[currPlayerPosition];
        Player prevPlayer = Manager.instance.playerOrderGame[prevPlayerPosition];
        prevPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, prevPlayer.name);

        prevPlayer.MakeMeCollector($"Golem", true);
        Manager.instance.instructions.text = $"Choose one for {currPlayer.name}'s Golem to ignore.";
        Collector x = prevPlayer.newCollector;

        x.AddText("Draw 1", true);
        x.AddText("Gain $5", true);
        x.AddText("Play 1", true);

        yield return prevPlayer.WaitForDecision();
        Destroy(x.gameObject);
        string ignoreThis = prevPlayer.choice;

        switch (ignoreThis)
        {
            case "Draw 1":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{prevPlayer.name} chooses \"draw 1.\"");
                this.pv.RPC("ExecuteChoice", prevPlayer.photonplayer, currPlayerPosition, "Draw 1");
                break;
            case "Gain $5":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{prevPlayer.name} chooses \"gain $5.\"");
                this.pv.RPC("ExecuteChoice", prevPlayer.photonplayer, currPlayerPosition, "Gain 5");
                break;
            case "Play 1":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{prevPlayer.name} chooses \"play 1 card.\"");
                this.pv.RPC("ExecuteChoice", prevPlayer.photonplayer, currPlayerPosition, "Play 1");
                break;
        }
    }

    [PunRPC]
    public IEnumerator ExecuteChoice(int currPlayerPosition, string choice)
    {
        Player currPlayer = Manager.instance.playerOrderGame[currPlayerPosition];

        switch (choice)
        {
            case "Draw 1":
                currPlayer.TryToGain(5);
                yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Golem");
                break;
            case "Gain $5":
                currPlayer.TryToDraw(1);
                yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Golem");
                break;
            case "Play 1":
                currPlayer.TryToDraw(1);
                currPlayer.TryToGain(5);
                break;
        }

        currPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[currPlayerPosition]);
    }
}
