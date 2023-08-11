using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Necromancer : PlayerCard
{
    List<PlayerCard> listOfCards = new List<PlayerCard>();

    public override void Setup()
    {
        logName = "a Necromancer";
        myColor = CardColor.Blue;
        myCost = 5;
        myCrowns = 2;
        director = true;
    }

    public override IEnumerator NowCommand(Player currPlayer)
    {
        currPlayer.MakeMeCollector("Necromancer", false);
        Collector x = currPlayer.newCollector;
        this.pv.RPC("RequestDraw", RpcTarget.MasterClient);
        yield return new WaitForSeconds(0.5f);

        Manager.instance.instructions.text = $"Command one of these twice.";
        for (int i = 0; i < listOfCards.Count; i++)
        {
            x.pv.RPC("AddCard", RpcTarget.All, listOfCards[i].pv.ViewID, true);
            yield return listOfCards[i].DiscardEffect(currPlayer, 0);
        }

        yield return currPlayer.WaitForDecision();

        PlayerCard commandMe = currPlayer.chosencard.GetComponent<PlayerCard>();
        x.pv.RPC("DestroyOtherButtons", RpcTarget.All, commandMe.pv.ViewID);

        yield return commandMe.InitialCommand(currPlayer, this);
        yield return commandMe.InitialCommand(currPlayer, this);

        PhotonNetwork.Destroy(x.pv);
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
