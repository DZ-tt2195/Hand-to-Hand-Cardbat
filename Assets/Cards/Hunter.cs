using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Hunter : PlayerCard
{
    public override void Setup()
    {
        logName = "a Hunter";
        myColor = CardColor.Gold;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.TryToGain(3);

        currPlayer.MakeMeCollector($"Hunter", true);
        Manager.instance.instructions.text = $"Choose a color for Hunter";
        Collector x = currPlayer.newCollector;

        x.AddText("Blue", true);
        x.AddText("Gold", true);
        x.AddText("Red", true);

        yield return currPlayer.WaitForDecision();
        Destroy(x.gameObject);
        string chosenColor = currPlayer.choice;
        Log.instance.pv.RPC("AddText", RpcTarget.All, $"{currPlayer.name} chooses {chosenColor}.");

        int playertracker = currPlayer.playerposition;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player nextPlayer = Manager.instance.playerOrderGame[playertracker];
            Manager.instance.instructions.text = $"Waiting for {nextPlayer.name}...";
            currPlayer.waiting = true;
            this.pv.RPC("DiscardOfColor", nextPlayer.photonplayer, nextPlayer.playerposition, currPlayer.playerposition, chosenColor);

            while (currPlayer.waiting)
                yield return null;

            playertracker = (playertracker == Manager.instance.playerOrderGame.Count - 1) ? 0 : playertracker + 1;
        }
    }

    [PunRPC]
    public IEnumerator DiscardOfColor(int thisPlayerPosition, int requestingPlayerPosition, string choice)
    {
        Player requestingPlayer = Manager.instance.playerOrderGame[requestingPlayerPosition];
        Player thisPlayer = Manager.instance.playerOrderGame[thisPlayerPosition];
        thisPlayer.pv.RPC("WaitForPlayer", RpcTarget.Others, thisPlayer.name);

        PlayerCard.CardColor chosenColor = PlayerCard.CardColor.None;
        switch (choice)
        {
            case "Blue":
                chosenColor = CardColor.Blue;
                break;
            case "Gold":
                chosenColor = CardColor.Gold;
                break;
            case "Red":
                chosenColor = CardColor.Red;
                break;
        }

        List<PlayerCard> possibleCards = new List<PlayerCard>();
        for (int i = 0; i < thisPlayer.listOfHand.Count; i++)
        {
            if (thisPlayer.listOfHand[i].myColor == chosenColor)
                possibleCards.Add(thisPlayer.listOfHand[i]);
        }

        if (possibleCards.Count >= 1)
        {
            Manager.instance.instructions.text = $"Discard a {choice} card with {requestingPlayer.name}'s Hunter?";
            thisPlayer.MakeMeCollector($"{requestingPlayer.name}'s Hunter", true);
            for (int i = 0; i < possibleCards.Count; i++)
                possibleCards[i].choicescript.EnableButton(thisPlayer, true);
            Collector x = thisPlayer.newCollector;
            x.pv.RPC("AddText", RpcTarget.All, "Decline", true);

            yield return thisPlayer.WaitForDecision();

            for (int i = 0; i < possibleCards.Count; i++)
                possibleCards[i].choicescript.DisableButton();
            Destroy(x.gameObject);

            if (thisPlayer.chosencard != null)
            {
                yield return thisPlayer.chosencard.GetComponent<PlayerCard>().DiscardEffect(thisPlayer, 1);
                thisPlayer.TryToGain(4);
            }
        }
        requestingPlayer.pv.RPC("WaitDone", Manager.instance.playerOrderPhoton[requestingPlayerPosition]);
    }
}
