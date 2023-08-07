using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Familiar : PlayerCard
{
    List<PlayerCard> listOfCards = new List<PlayerCard>();

    public override void Setup()
    {
        logName = "a Familiar";
        myColor = CardColor.Gold;
        myCost = 0;
        myCrowns = 1;
        director = false;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.MakeMeCollector("Familiar", false);
        Collector x = currPlayer.newCollector;

        currPlayer.MakeMeCollector("Play a card?", true);
        Collector y = currPlayer.newCollector;
        y.AddText("Decline", true);

        this.pv.RPC("RequestDraw", RpcTarget.MasterClient);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < listOfCards.Count; i++)
        {
            x.pv.RPC("AddCard", RpcTarget.All, listOfCards[i].pv.ViewID, listOfCards[i].CanPlayThis(currPlayer));
        }

        yield return currPlayer.WaitForDecision();

        Destroy(y.gameObject);
        PhotonNetwork.Destroy(x.pv);

        if (currPlayer.chosencard != null)
        {
            currPlayer.pv.RPC("TakeCrown", RpcTarget.All, 1);
            currPlayer.pv.RPC("PlayCard", RpcTarget.All, currPlayer.chosencard.pv.ViewID);
        }
    }

    [PunRPC]
    void RequestDraw()
    {
        int[] cardIDs = new int[3];

        for (int i = 0; i < cardIDs.Length; i++)
        {
            if (Manager.instance.deck.childCount == 0)
            {
                Manager.instance.discard.Shuffle();
                while (Manager.instance.discard.childCount > 0)
                    Manager.instance.discard.GetChild(0).SetParent(Manager.instance.deck);
            }

            PhotonView x = Manager.instance.deck.GetChild(0).GetComponent<PhotonView>();
            cardIDs[i] = x.ViewID;
            x.transform.SetParent(null);
        }

        this.pv.RPC("SendDraw", RpcTarget.All, cardIDs);
    }

    [PunRPC]
    void SendDraw(int[] cardIDs)
    {
        listOfCards.Clear();
        for (int i = 0; i < cardIDs.Length; i++)
        {
            PlayerCard nextcard = PhotonView.Find(cardIDs[i]).gameObject.GetComponent<PlayerCard>();
            listOfCards.Add(nextcard);
        }
    }
}
