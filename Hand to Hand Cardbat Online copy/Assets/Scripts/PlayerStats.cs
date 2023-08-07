using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Linq;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]public PhotonView pv;
    Player myPlayer;

    public int score;
    public Transform cardHand;
    public List<PlayerCard> listOfHand = new List<PlayerCard>();

    public Transform cardPlay;
    public List<PlayerCard> listOfPlay = new List<PlayerCard>();

    public int coins = 5;
    public int negCrowns = 0;

    private void Awake()
    {
        myPlayer = GetComponent<Player>();
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        cardHand.transform.localPosition = new Vector2(0, -230);
    }

    [PunRPC]
    public void RequestDraw(int cardsToDraw)
    {
        int[] cardIDs = new int[cardsToDraw];

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

        pv.RPC("SendDraw", RpcTarget.All, cardIDs);
    }

    [PunRPC]
    IEnumerator SendDraw(int[] cardIDs)
    {
        for (int i = 0; i < cardIDs.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            PlayerCard nextCard = PhotonView.Find(cardIDs[i]).gameObject.GetComponent<PlayerCard>();
            nextCard.image.sprite = (this.pv.AmOwner) ? nextCard.originalImage : Manager.instance.hiddenCard;

            if (this.cardHand.childCount == 0)
            {
                nextCard.transform.SetParent(this.cardHand);
                listOfHand.Add(nextCard);
            }

            else if (Manager.instance.sorting == Manager.Sorting.color)
                AddCardByColor(nextCard, 0, cardHand.childCount - 1, true);
            else
                AddCardByCost(nextCard, 0, cardHand.childCount - 1);
        }

        myPlayer.pv.RPC("UpdateButtonText", RpcTarget.All);
    }

    void AddCardByColor(PlayerCard nextCard, int low, int high, bool hand)
    {
        if (high <= low)
        {
            PlayerCard lowCard = cardHand.GetChild(low).GetComponent<PlayerCard>();

            if (nextCard.suitCode > lowCard.suitCode)
            {
                if (hand)
                {
                    listOfHand.Insert(low + 1, nextCard);
                    nextCard.transform.SetParent(cardHand.transform);
                    nextCard.transform.SetSiblingIndex(low + 1);
                    return;
                }
                else
                {
                    listOfPlay.Insert(low + 1, nextCard);
                    nextCard.transform.SetParent(cardPlay.transform);
                    nextCard.transform.SetSiblingIndex(low + 1);
                    return;
                }
            }
            else
            {
                if (hand)
                {
                    listOfHand.Insert(low, nextCard);
                    nextCard.transform.SetParent(cardHand.transform);
                    nextCard.transform.SetSiblingIndex(low);
                    return;
                }
                else
                {
                    listOfPlay.Insert(low, nextCard);
                    nextCard.transform.SetParent(cardPlay.transform);
                    nextCard.transform.SetSiblingIndex(low);
                    return;
                }
            }
        }

        int mid = (low + high) / 2;
        PlayerCard midCard = cardHand.GetChild(mid).GetComponent<PlayerCard>();

        if (nextCard.suitCode == midCard.suitCode)
        {
            listOfHand.Insert(mid + 1, nextCard);
            nextCard.transform.SetParent(cardHand.transform);
            nextCard.transform.SetSiblingIndex(mid + 1);
            return;
        }

        if (nextCard.suitCode > midCard.suitCode)
            AddCardByColor(nextCard, mid + 1, high, hand);
        else
            AddCardByColor(nextCard, low, mid - 1, hand);
    }

    void AddCardByCost(PlayerCard nextCard, int low, int high)
    {
        if (high <= low)
        {
            PlayerCard lowCard = cardHand.GetChild(low).GetComponent<PlayerCard>();

            if (nextCard.myCost > lowCard.myCost)
            {
                listOfHand.Insert(low + 1, nextCard);
                nextCard.transform.SetParent(cardHand.transform);
                nextCard.transform.SetSiblingIndex(low + 1);
                return;
            }
            else
            {
                listOfHand.Insert(low, nextCard);
                nextCard.transform.SetParent(cardHand.transform);
                nextCard.transform.SetSiblingIndex(low);
                return;
            }
        }

        int mid = (low + high) / 2;
        PlayerCard midCard = cardHand.GetChild(mid).GetComponent<PlayerCard>();

        if (nextCard.myCost == midCard.myCost)
        {
            listOfHand.Insert(mid + 1, nextCard);
            nextCard.transform.SetParent(cardHand.transform);
            nextCard.transform.SetSiblingIndex(mid + 1);
            return;
        }

        if (nextCard.myCost > midCard.myCost)
            AddCardByCost(nextCard, mid + 1, high);
        else
            AddCardByCost(nextCard, low, mid - 1);
    }

    public void SortHandByColor()
    {
        listOfHand = listOfHand.OrderBy(o => o.suitCode).ToList();
        for (int i = 0; i < listOfHand.Count; i++)
        {
            PhotonView.Find(listOfHand[i].pv.ViewID).transform.SetSiblingIndex(i);
        }
    }

    public void SortHandByCost()
    {
        listOfHand = listOfHand.OrderBy(o => o.myCost).ToList();
        for (int i = 0; i < listOfHand.Count; i++)
        {
            PhotonView.Find(listOfHand[i].pv.ViewID).transform.SetSiblingIndex(i);
        }
    }


    [PunRPC]
    public IEnumerator GainCoin(int n)
    {
        yield return new WaitForSeconds(0.5f);
        coins += n;
        myPlayer.UpdateButtonText();
    }

    [PunRPC]
    public IEnumerator LoseCoin(int n)
    {
        yield return new WaitForSeconds(0.5f);
        coins -= n;
        if (coins <= 0)
            coins = 0;
        myPlayer.UpdateButtonText();
    }

    [PunRPC]
    public IEnumerator TakeCrown(int n)
    {
        yield return new WaitForSeconds(0.5f);
        negCrowns += n;
        myPlayer.UpdateButtonText();
    }

    [PunRPC]
    public IEnumerator LoseCrown(int n)
    {
        yield return new WaitForSeconds(0.5f);
        negCrowns -= n;
        if (negCrowns <= 0)
            negCrowns = 0;
        myPlayer.UpdateButtonText();
    }

    [PunRPC]
    public void PutInDiscard(int cardID, int code)
    {
        PlayerCard discardMe = PhotonView.Find(cardID).GetComponent<PlayerCard>();
        myPlayer.cardsDiscardedThisTurn++;
        this.transform.SetParent(Manager.instance.discard);

        switch (code)
        {
            case 0:
                break;
            case 1:
                listOfHand.Remove(discardMe);
                break;
            case 2:
                listOfPlay.Remove(discardMe);
                break;
        }

        myPlayer.UpdateButtonText();
    }

    public void CalculateScore()
    {
        int totalScore = negCrowns;
        for (int i = 0; i < listOfPlay.Count; i++)
            totalScore += listOfPlay[i].myCrowns;
    }

    [PunRPC]
    public IEnumerator PlayCard(int cardID)
    {
        PlayerCard newCard = PhotonView.Find(cardID).GetComponent<PlayerCard>();
        newCard.image.sprite = newCard.originalImage;
        listOfHand.Remove(newCard);
        newCard.UnRotateMe();
        myPlayer.cardsPlayedThisTurn.Add(newCard);
        AddCardByColor(newCard, 0, cardPlay.childCount - 1, false);
        yield return LoseCoin(newCard.myCost);

        if (this.pv.AmOwner)
        {
            yield return newCard.PlayEffect(myPlayer);
        }
    }

    [PunRPC]
    public IEnumerator FreePlayMe(PlayerCard newCard)
    {
        newCard.UnRotateMe();
        myPlayer.cardsPlayedThisTurn.Add(newCard);
        AddCardByColor(newCard, 0, cardPlay.childCount - 1, false);
        //yield return LoseCoin(newCard.myCost);

        if (this.pv.AmOwner)
        {
            yield return newCard.PlayEffect(myPlayer);
        }
    }
}
