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

public class Player : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public int score;

    public RectTransform cardHand;
    public List<PlayerCard> listOfHand = new List<PlayerCard>();

    public RectTransform cardPlay;
    public List<PlayerCard> listOfPlay = new List<PlayerCard>();

    public int coins = 5;
    public int negCrowns = 0;

    public int playerposition;
    public Photon.Realtime.Player photonplayer;
    Button resign;

    [HideInInspector] public PlayerButton myButton;
    [HideInInspector] Transform arrow;
    Transform storePlayers;
    int movePosition;

    public bool waiting;
    public bool turnon;

    [HideInInspector] public string choice;
    [HideInInspector] public Card chosencard;
    [HideInInspector] public SendChoice chosenbutton;

    [HideInInspector] public Collector newCollector;

    public enum Actions { None, Explore, Collect, Recruit, Unleash};
    public Actions lastUsedAction;
    public Actions currentAction;

    public List<PlayerCard> cardsPlayedThisTurn;
    public int merchantDebt;

    public enum TypeOfTurn { Normal, MeteorShower, Sphinx};
    [HideInInspector] public TypeOfTurn thisTurn;
    [HideInInspector] public int meteorShowerTurn;
    [HideInInspector] public int sphinxTurn;

    private void Awake()
    {
        resign = GameObject.Find("Resign Button").GetComponent<Button>();
        storePlayers = GameObject.Find("Store Players").transform;
        arrow = GameObject.Find("Arrow").transform;
    }

    private void Start()
    {
        cardsPlayedThisTurn = new List<PlayerCard>();
        lastUsedAction = Actions.None;

        if (this.pv.AmController)
            resign.onClick.AddListener(ResignTime);
        this.name = pv.Owner.NickName;
        this.gameObject.transform.SetParent(storePlayers);

    }

    public void ResignTime()
    {
        Manager.instance.GameOver($"{this.name} has resigned.", this.playerposition);
    }

    [PunRPC]
    public void AssignInfo(int position, Photon.Realtime.Player playername)
    {
        this.playerposition = position;
        this.transform.localPosition = new Vector3(0 + 2000 * playerposition, 0, 0);
        this.transform.localScale = new Vector3(1, 1, 1);
        this.photonplayer = playername;
        this.myButton = GameObject.Find($"{this.name}'s Button").GetComponent<PlayerButton>();
        myButton.myName.text = this.name;
        photonView.RPC("UpdateButtonText", RpcTarget.All);
        photonView.RPC("ButtonClick", RpcTarget.All);

        if (pv.AmOwner)
            ClickMe();
    }

    [PunRPC]
    public void UpdateButtonText()
    {
        myButton.myCards.text = $"{listOfHand.Count}";
        myButton.myCoins.text = $"{coins}";
        myButton.myCrowns.text = $"-{negCrowns}";
    }

    [PunRPC]
    public void ButtonClick()
    {
        movePosition = -2000 * playerposition;
        myButton.button.onClick.AddListener(ClickMe);
    }

    public void ClickMe()
    {
        storePlayers.transform.localPosition = new Vector3(movePosition, 0, 0);
        arrow.transform.SetParent(myButton.transform);
        arrow.transform.localPosition = new Vector3(95.5f, 0, 0);
    }

    public Player GetPreviousPlayer()
    {
        if (this.playerposition == 0)
            return Manager.instance.playerOrderGame[^1];
        else
            return Manager.instance.playerOrderGame[this.playerposition - 1];
    }

    public void TryToDraw(int cardsToDraw)
    {
        if (Manager.instance.EventActive("Masquerade") && Manager.instance.currentPlayer == this)
        {
            GetPreviousPlayer().pv.RPC("RequestDraw", RpcTarget.MasterClient, cardsToDraw);
            Log.instance.AddText($"{this.name} would draw {cardsToDraw} cards.");
        }
        else
        {
            this.pv.RPC("RequestDraw", RpcTarget.MasterClient, cardsToDraw);
        }
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

        photonView.RPC("SendDraw", RpcTarget.All, cardIDs, false);
    }

    [PunRPC]
    public IEnumerator SendDraw(int[] cardIDs, bool publicReturn)
    {
        for (int i = 0; i < cardIDs.Length; i++)
        {
            yield return new WaitForSeconds(0.03f);
            PlayerCard nextCard = PhotonView.Find(cardIDs[i]).gameObject.GetComponent<PlayerCard>();
            nextCard.pv.RPC("UnRotateMe", RpcTarget.All);

            if (publicReturn)
            {
                Log.instance.AddText($"{this.name} returns {nextCard.logName} to their hand.");
                listOfPlay.Remove(nextCard);
            }
            else if (this.pv.AmOwner)
            {
                Log.instance.AddText($"{this.name} draws {nextCard.logName}.");
            }
            else
            {
                Log.instance.AddText($"{this.name} draws a card.");
            }

            nextCard.image.sprite = (this.pv.AmOwner) ? nextCard.originalImage : Manager.instance.hiddenCard;

            if (Manager.instance.sorting == Manager.Sorting.color)
                AddCardByColor(nextCard, 0, cardHand.childCount - 1, true);
            else
                AddCardByCost(nextCard, 0, cardHand.childCount - 1);
        }

        photonView.RPC("UpdateButtonText", RpcTarget.All);
    }

    public void AddCardByColor(PlayerCard nextCard, int low, int high, bool hand)
    {
        if (hand && this.listOfHand.Count == 0)
        {
            nextCard.transform.SetParent(this.cardHand);
            listOfHand.Add(nextCard);
            return;
        }
        else if (!hand && this.listOfPlay.Count == 0)
        {
            nextCard.transform.SetParent(this.cardPlay);
            listOfPlay.Add(nextCard);
            return;
        }

        if (high <= low)
        {
            PlayerCard lowCard = cardHand.GetChild(low).GetComponent<PlayerCard>();

            if (nextCard.suitCode >= lowCard.suitCode)
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
            if (hand)
            {
                listOfHand.Insert(mid + 1, nextCard);
                nextCard.transform.SetParent(cardHand.transform);
                nextCard.transform.SetSiblingIndex(mid + 1);
                return;
            }
            else
            {
                listOfPlay.Insert(mid + 1, nextCard);
                nextCard.transform.SetParent(cardPlay.transform);
                nextCard.transform.SetSiblingIndex(mid + 1);
                return;
            }
        }

        if (nextCard.suitCode > midCard.suitCode)
            AddCardByColor(nextCard, mid + 1, high, hand);
        else
            AddCardByColor(nextCard, low, mid - 1, hand);
    }

    void AddCardByCost(PlayerCard nextCard, int low, int high)
    {
        if (listOfHand.Count == 0)
        {
            listOfHand.Add(nextCard);
            nextCard.transform.SetParent(cardHand.transform);
            return;
        }

        if (high <= low)
        {
            PlayerCard lowCard = cardHand.GetChild(low).GetComponent<PlayerCard>();

            if (nextCard.myCost >= lowCard.myCost)
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

    public void TryToGain(int coinsToGain)
    {
        if (Manager.instance.EventActive("Masquerade") && Manager.instance.currentPlayer == this)
        {
            Log.instance.AddText($"{this.name} would gain ${coinsToGain}.");
            GetPreviousPlayer().pv.RPC("GainCoin", RpcTarget.All, coinsToGain);
        }
        else
        {
            this.pv.RPC("GainCoin", RpcTarget.All, coinsToGain);
        }
    }

    [PunRPC]
    public IEnumerator GainCoin(int n)
    {
        if (n > 0)
        {
            yield return new WaitForSeconds(0.25f);
            Log.instance.AddText($"{this.name} gains ${n}.");
            coins += n;
            UpdateButtonText();
        }
    }

    [PunRPC]
    public IEnumerator LoseCoin(int n)
    {
        if (n > 0)
        {
            yield return new WaitForSeconds(0.25f);
            Log.instance.AddText($"{this.name} loses ${n}.");
            coins -= n;
            if (coins <= 0)
                coins = 0;
            UpdateButtonText();
        }
    }

    [PunRPC]
    public IEnumerator TakeCrown(int n)
    {
        if (n > 0)
        {
            yield return new WaitForSeconds(0.25f);
            Log.instance.AddText($"{this.name} takes -{n} Crowns.");
            negCrowns += n;
            UpdateButtonText();
        }
    }

    [PunRPC]
    public IEnumerator LoseCrown(int n)
    {
        if (n > 0)
        {
            yield return new WaitForSeconds(0.25f);
            Log.instance.AddText($"{this.name} removes -{n} Crowns.");
            negCrowns -= n;
            if (negCrowns <= 0)
                negCrowns = 0;
            UpdateButtonText();
        }
    }

    [PunRPC]
    public IEnumerator WaitForPlayer(string playername)
    {
        waiting = true;
        Manager.instance.instructions.text = $"Waiting for {playername}...";
        while (waiting)
        {
            yield return null;
        }
    }

    [PunRPC]
    public void WaitDone()
    {
        Debug.Log("Waiting has ended");
        waiting = false;
    }

    public IEnumerator TakeTurnRPC(Photon.Realtime.Player requestingplayer)
    {
        pv.RPC("TurnStart", RpcTarget.All);
        pv.RPC("TakeTurn", requestingplayer, TypeOfTurn.Normal);
        while (turnon)
            yield return null;
    }

    [PunRPC]
    void TurnStart()
    {
        turnon = true;
        Manager.instance.currentPlayer = this;
    }

    [PunRPC]
    void TurnOver()
    {
        turnon = false;
    }

    IEnumerator ResolveEvents(Event.EventTrigger x)
    {
        List<Event> toResolve = new List<Event>();
        this.MakeMeCollector($"{this.name}'s Turn", false);
        Collector y = newCollector;

        for (int i = 0; i<Manager.instance.chosenEvents.Count; i++)
        {
            Event nextEvent = Manager.instance.chosenEvents[i];
            if (nextEvent.thisTrigger == x && nextEvent.IsActive(Manager.instance.turnNumber))
            {
                toResolve.Add(Manager.instance.chosenEvents[i]);
                y.pv.RPC("AddCard", RpcTarget.All, Manager.instance.chosenEvents[i].pv.ViewID, true);
            }
        }

        if (toResolve.Count >= 1)
        {
            Manager.instance.instructions.text = "Resolve an Event.";
            for (int i = 0; i < toResolve.Count; i++)
            {
                pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);
                y.EnableAll();
                yield return this.WaitForDecision();

                y.DisableAll();
                y.pv.RPC("DestroyButton", RpcTarget.All, this.chosenbutton.transform.GetSiblingIndex());

                Log.instance.pv.RPC("AddText", RpcTarget.All, $"");
                Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name} resolves {chosencard.logName}.");
                yield return this.chosencard.GetComponent<Event>().UseEvent(this);
            }
        }
        if (y != null)
        {
            PhotonNetwork.Destroy(y.pv);
        }
    }

    [PunRPC]
    public IEnumerator TakeTurn(TypeOfTurn thisTurn)
    {
        if (pv.IsMine)
        {
            this.thisTurn = thisTurn;
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"");

            switch (thisTurn)
            {
                case TypeOfTurn.Normal:
                    Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name}'s Turn");
                    break;
                case TypeOfTurn.MeteorShower:
                    Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name}'s Extra Turn (Meteor Shower)");
                    break;
                case TypeOfTurn.Sphinx:
                    Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name}'s Extra Turn (Sphinx)");
                    break;
            }

            ClickMe();
            pv.RPC("TurnStart", RpcTarget.All);
            pv.RPC("WaitForPlayer", RpcTarget.Others, this.name);

            yield return ResolveEvents(Event.EventTrigger.TurnStart);

            yield return ChooseYourAction();

            choice = "";
            chosencard = null;
            chosenbutton = null;

            yield return ResolveEvents(Event.EventTrigger.TurnEnd);

            for (int i = 0; i<Manager.instance.playerOrderGame.Count; i++)
                Manager.instance.playerOrderGame[i].pv.RPC("ResetPlayerThings", RpcTarget.All);

            if (meteorShowerTurn > 0)
            {
                meteorShowerTurn--;
                yield return TakeTurn(TypeOfTurn.MeteorShower);
            }
            else if (sphinxTurn > 0)
            {
                sphinxTurn--;
                yield return TakeTurn(TypeOfTurn.Sphinx);
            }
            else
            {
                photonView.RPC("TurnOver", RpcTarget.All);
            }
        }
    }

    IEnumerator ChooseYourAction()
    {
        Log.instance.pv.RPC("AddText", RpcTarget.All, $"");

        if (Manager.instance.EventActive("Job Fair"))
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name} has to use Recruit.");
            yield return Manager.instance.listOfActions[2].UseAction(this);
        }
        else
        {
            this.MakeMeCollector($"{this.name}'s Turn", false);
            Collector x = newCollector;

            Manager.instance.instructions.text = $"Choose an Action";
            for (int i = 0; i < Manager.instance.listOfActions.Count; i++)
            {
                if (Manager.instance.listOfActions[i].name == "Unleash" && thisTurn == TypeOfTurn.MeteorShower)
                    x.pv.RPC("AddCard", RpcTarget.All, Manager.instance.listOfActions[i].pv.ViewID, false);
                else
                    x.pv.RPC("AddCard", RpcTarget.All, Manager.instance.listOfActions[i].pv.ViewID, true);
            }
            yield return WaitForDecision();

            PhotonNetwork.Destroy(x.pv);
            Action chosenAction = this.chosencard.GetComponent<Action>();
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{this.name} uses {chosenAction.name}.");
            yield return chosenAction.UseAction(this);
        }
    }

    [PunRPC]
    public IEnumerator ResetPlayerThings()
    {
        lastUsedAction = currentAction;
        currentAction = Actions.None;
        cardsPlayedThisTurn.Clear();

        if (merchantDebt > 0)
        {
            yield return LoseCoin(merchantDebt);
            merchantDebt = 0;
        }
        UpdateButtonText();
    }

    public IEnumerator WaitForDecision()
    {
        choice = "";
        chosencard = null;
        chosenbutton = null;

        while (choice == "")
            yield return null;
    }

    public IEnumerator ChooseToPlay(List<PlayerCard> cardsToChoose, string source, bool forFree)
    {
        if (cardsToChoose.Count > 0)
        {
            int canBePlayed = 0;
            for (int i = 0; i < cardsToChoose.Count; i++)
            {
                if (forFree || cardsToChoose[i].CanPlayThis(this))
                {
                    canBePlayed++;
                    cardsToChoose[i].choicescript.EnableButton(this, true);
                }
            }

            if (canBePlayed > 0)
            {
                yield return new WaitForSeconds(0.25f);
                this.MakeMeCollector($"{source}", true);
                Collector x = newCollector;
                x.AddText("Decline", true);

                if (forFree)
                    Manager.instance.instructions.text = $"Play a card for free with {source}?";
                else
                    Manager.instance.instructions.text = $"Play a card with {source}?";

                yield return WaitForDecision();

                for (int i = 0; i < cardsToChoose.Count; i++)
                    cardsToChoose[i].choicescript.DisableButton();
                Destroy(x.gameObject);

                if (choice != "Decline")
                {
                    if (forFree)
                        this.pv.RPC("FreePlayMe", RpcTarget.All, chosencard.pv.ViewID);
                    else
                        this.pv.RPC("PlayCard", RpcTarget.All, chosencard.pv.ViewID);

                    waiting = true;
                    while (waiting)
                        yield return null;
                }
            }
        }
    }

    public IEnumerator ChooseToPlay(List<PlayerCard> cardsToChoose, string source)
    {
        yield return ChooseToPlay(cardsToChoose, source, false);
    }

    [PunRPC]
    public IEnumerator PlayCard(int cardID)
    {
        PlayerCard newCard = PhotonView.Find(cardID).GetComponent<PlayerCard>();
        newCard.image.sprite = newCard.originalImage;

        listOfHand.Remove(newCard);
        newCard.UnRotateMe();
        cardsPlayedThisTurn.Add(newCard);
        AddCardByColor(newCard, 0, cardPlay.childCount - 1, false);

        yield return LoseCoin(newCard.myCost);
        yield return new WaitForSeconds(0.25f);
        Log.instance.AddText($"{this.name} plays {newCard.logName}.");

        if (this.pv.AmOwner)
        {
            yield return newCard.PlayEffect(this);
            this.WaitDone();
        }
    }

    [PunRPC]
    public IEnumerator FreePlayMe(int cardID)
    {
        PlayerCard newCard = PhotonView.Find(cardID).GetComponent<PlayerCard>();
        newCard.image.sprite = newCard.originalImage;

        listOfHand.Remove(newCard);
        newCard.UnRotateMe();
        cardsPlayedThisTurn.Add(newCard);
        AddCardByColor(newCard, 0, cardPlay.childCount - 1, false);

        //yield return LoseCoin(newCard.myCost);
        yield return new WaitForSeconds(0.25f);
        Log.instance.AddText($"{this.name} plays {newCard.logName} for free.");

        if (this.pv.AmOwner)
        {
            yield return newCard.PlayEffect(this);
            this.WaitDone();
        }
    }

    public void MakeMeCollector(string itsText, bool textCollector)
    {
        if (textCollector)
        {
            newCollector = Instantiate((Manager.instance.textCollector));
            newCollector.StatsSetup(itsText, this.playerposition, -150);
        }
        else //if card collector
        {
            GameObject nc = PhotonNetwork.Instantiate(Manager.instance.cardCollector.name, new Vector3(0, -200, 0), Quaternion.identity);
            newCollector = nc.GetComponent<Collector>();
            newCollector.pv.RPC("StatsSetup", RpcTarget.All, itsText, this.playerposition, 150);
        }
    }

    [PunRPC]
    public void PutInDiscard(int cardID, int code)
    {
        PlayerCard discardMe = PhotonView.Find(cardID).GetComponent<PlayerCard>();
        discardMe.transform.SetParent(Manager.instance.discard);
        discardMe.transform.localPosition = discardMe.originalPos;

        switch (code)
        {
            case 0:
                Log.instance.AddText($"{this.name} discards {discardMe.logName} from the deck.");
                break;
            case 1:
                Log.instance.AddText($"{this.name} discards {discardMe.logName} from their hand.");
                listOfHand.Remove(discardMe);
                break;
            case 2:
                Log.instance.AddText($"{this.name} discards {discardMe.logName} from their play area.");
                listOfPlay.Remove(discardMe);
                break;
        }

        if (this.pv.AmOwner && Manager.instance.EventActive("Cleaning"))
            this.TryToGain(1);

        UpdateButtonText();
    }

    public void CalculateScore()
    {
        int totalScore = negCrowns;
        for (int i = 0; i < listOfPlay.Count; i++)
            totalScore += listOfPlay[i].myCrowns;
    }
}