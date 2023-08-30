using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Advisor : PlayerCard
{
    bool stillResolving = true;

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
        stillResolving = true;
        Player prevPlayer = currPlayer.GetPreviousPlayer();
        this.pv.RPC("MakeChoice", prevPlayer.photonplayer, prevPlayer.playerposition, currPlayer.playerposition);

        while (stillResolving)
            yield return null;
    }

    [PunRPC]
    public IEnumerator MakeChoice(int prevPlayerPosition, int currPlayerPosition)
    {
        yield return null;
        Player currPlayer = Manager.instance.playerOrderGame[currPlayerPosition];
        Player prevPlayer = Manager.instance.playerOrderGame[prevPlayerPosition];
        prevPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, prevPlayer.name);

        prevPlayer.MakeMeCollector($"Advisor", true);
        Manager.instance.instructions.text = $"Choose one for {currPlayer.name}'s Advisor to do.";
        Collector x = prevPlayer.newCollector;

        x.AddText("Draw 3", true);
        x.AddText("Gain $12", true);
        x.AddText("Play 2", true);

        yield return prevPlayer.WaitForDecision();
        Destroy(x.gameObject);
        string ignoreThis = prevPlayer.choice;

        switch (ignoreThis)
        {
            case "Draw 3":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{prevPlayer.name} chooses \"draw 3.\"");
                this.pv.RPC("ExecuteChoice", currPlayer.photonplayer, currPlayerPosition, "Draw 3");
                break;
            case "Gain $12":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{prevPlayer.name} chooses \"gain $12.\"");
                this.pv.RPC("ExecuteChoice", currPlayer.photonplayer, currPlayerPosition, "Gain 12");
                break;
            case "Play 2":
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{prevPlayer.name} chooses \"play 2 cards.\"");
                this.pv.RPC("ExecuteChoice", currPlayer.photonplayer, currPlayerPosition, "Play 2");
                break;
        }
    }

    [PunRPC]
    public IEnumerator ExecuteChoice(int currPlayerPosition, string choice)
    {
        Player currPlayer = Manager.instance.playerOrderGame[currPlayerPosition];

        switch (choice)
        {
            case "Draw 3":
                currPlayer.TryToDraw(3);
                break;
            case "Gain $12":
                currPlayer.TryToGain(12);
                break;
            case "Play 2":
                yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Advisor");
                yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Advisor");
                break;
        }

        currPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[currPlayerPosition]);
        pv.RPC("Finished", RpcTarget.All);
    }

    [PunRPC]
    public void Finished()
    {
        stillResolving = false;
    }
}
