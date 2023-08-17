using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Linq;
using System;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour, IOnEventCallback
{
    [HideInInspector] public Player currentPlayer;
    public static Manager instance;
    public Sprite hiddenCard;

    [HideInInspector] public Canvas canvas;
    public Collector cardCollector;
    public Collector textCollector;

    public List<PlayerButton> playerButtonClone;
    [HideInInspector] public List<Action> listOfActions = new List<Action>();

    [HideInInspector] public TMP_Text instructions;
    [HideInInspector] public Transform deck;
    [HideInInspector] public Transform discard;

    [HideInInspector] public List<Player> playerOrderGame = new List<Player>();
    [HideInInspector] public List<Photon.Realtime.Player> playerOrderPhoton = new List<Photon.Realtime.Player>();

    [HideInInspector] public float opacity = 1;
    [HideInInspector] public bool decrease = true;
    [HideInInspector] public bool gameon = false;

    public enum Sorting { color, cost }
    public Sorting sorting = Sorting.color;

    [HideInInspector] public const byte AddNextPlayerEvent = 1;
    [HideInInspector] public const byte NextChosenEvent = 2;
    [HideInInspector] public const byte AdvanceTurnEvent = 3;
    [HideInInspector] public const byte GameOverEvent = 4;

    [HideInInspector] public Transform eventDeck;
    [HideInInspector] public Transform eventBar;
    [HideInInspector] public List<Event> chosenEvents = new List<Event>();

    public int turnNumber;
    [HideInInspector] public TMP_Text turnText;

    Button sortingButton;
    TMP_Text sortingText;
    public GameObject blownUp;
    public TMP_Text endText;
    public Button leaveRoom;

    private void FixedUpdate()
    {
        if (decrease)
            opacity -= 0.05f;
        else
            opacity += 0.05f;
        if (opacity < 0 || opacity > 1)
            decrease = !decrease;
    }

    void Awake()
    {
        instance = this;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        instructions = GameObject.Find("Instructions").GetComponent<TMP_Text>();
        eventBar = GameObject.Find("Event Bar").transform;
        turnText = GameObject.Find("Clock Time").GetComponent<TMP_Text>();

        eventDeck = GameObject.Find("Event Deck").transform;
        deck = GameObject.Find("Deck").transform;
        discard = GameObject.Find("Discard").transform;

        sortingButton = GameObject.Find("Sorting Button").GetComponent<Button>();
        sortingText = sortingButton.GetComponentInChildren<TMP_Text>();
        sortingButton.onClick.AddListener(ChangeSorting);

        listOfActions.Add(GameObject.Find("Explore").GetComponent<Action>());
        listOfActions.Add(GameObject.Find("Collect").GetComponent<Action>());
        listOfActions.Add(GameObject.Find("Recruit").GetComponent<Action>());
        listOfActions.Add(GameObject.Find("Unleash").GetComponent<Action>());
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            blownUp.SetActive(false);
    }

    public void ChangeSorting()
    {
        if (sorting == Sorting.color)
        {
            sorting = Sorting.cost;
            sortingText.text = "Sorted by cost";
            for (int i = 0; i < playerOrderGame.Count; i++)
            {
                playerOrderGame[i].SortHandByCost();
            }
        }
        else
        {
            sorting = Sorting.color;
            sortingText.text = "Sorted by color";
            for (int i = 0; i < playerOrderGame.Count; i++)
            {
                playerOrderGame[i].SortHandByColor();
            }
        }
    }

    void Start()
    {
        leaveRoom.onClick.AddListener(Quit);
        endText.transform.parent.gameObject.SetActive(false);
        blownUp.SetActive(false);
        StartCoroutine(WaitForPlayer());
    }

    public bool EventActive(string eventName)
    {
        for (int i = 0; i<chosenEvents.Count; i++)
        {
            if (eventName == chosenEvents[i].name)
                return chosenEvents[i].IsActive(turnNumber);
        }
        return false;
    }

    IEnumerator WaitForPlayer()
    {
        Transform x = GameObject.Find("Store Players").transform;
        while (x.childCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            instructions.text = $"Waiting for more players...({PhotonNetwork.PlayerList.Length}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
            yield return null;
        }

        instructions.text = "Everyone's in! Setting up...";

        yield return new WaitForSeconds(0.5f);
        ChangeSorting();

        if (PhotonNetwork.IsMasterClient)
        {
            yield return PlayGame();
        }
    }

    IEnumerator GetEvents()
    {
        eventDeck.Shuffle();
        int numEvents = 0;
        switch (LocationInfo.instance.dropValue)
        {
            case 0:
                numEvents = UnityEngine.Random.Range(0, 4);
                UnityEngine.Debug.Log($"random # of events: {numEvents}");
                break;
            case 1:
                numEvents = 0;
                break;
            case 2:
                numEvents = 1;
                break;
            case 3:
                numEvents = 2;
                break;
            case 4:
                numEvents = 3;
                break;
        }

        if (numEvents == 0)
            Log.instance.pv.RPC("AddText", RpcTarget.All, "Using 0 Events.");

        for (int i = 0; i<numEvents; i++)
        {
            PhotonView x = eventDeck.GetChild(0).GetComponent<PhotonView>();
            x.transform.SetParent(null);

            object[] sendingdata = new object[1];
            sendingdata[0] = x.ViewID;

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(NextChosenEvent, sendingdata, raiseEventOptions, SendOptions.SendReliable);

            yield return new WaitForSeconds(0.25f);
        }
        Log.instance.pv.RPC("AddText", RpcTarget.All, "");
    }

    IEnumerator GetPlayers()
    {
        List<Photon.Realtime.Player> playerAssignment = new List<Photon.Realtime.Player>();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            playerAssignment.Add(PhotonNetwork.PlayerList[i]);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            object[] sendingdata = new object[2];
            sendingdata[0] = i;

            int randomremove = UnityEngine.Random.Range(0, playerAssignment.Count);
            sendingdata[1] = playerAssignment[randomremove];
            playerAssignment.RemoveAt(randomremove);

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(AddNextPlayerEvent, sendingdata, raiseEventOptions, SendOptions.SendReliable);
        }

        yield return new WaitForSeconds(0.5f);
        deck.Shuffle();
        for (int i = 0; i < playerOrderGame.Count; i++)
        {
            playerOrderGame[i].pv.RPC("RequestDraw", RpcTarget.MasterClient, 4);
        }
    }

    IEnumerator PlayGame()
    {
        Log.instance.pv.RPC("AddText", RpcTarget.All, $"SETUP");
        yield return GetEvents();
        yield return GetPlayers();

        yield return new WaitForSeconds(1f);
        gameon = true;

        object[] sendingdata = new object[1];
        sendingdata[0] = 0;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(AdvanceTurnEvent, sendingdata, raiseEventOptions, SendOptions.SendReliable);

        while (gameon)
        {
            yield return new WaitForSeconds(0.5f);
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"");
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"ROUND {turnNumber+1}");
            if (turnNumber == 0)
                Log.instance.pv.RPC("AddText", RpcTarget.All, "You can't command cards this round.");

            for (int i = 0; i<chosenEvents.Count; i++)
            {
                if (chosenEvents[i].IsActive(turnNumber))
                    Log.instance.pv.RPC("AddText", RpcTarget.All, $"{chosenEvents[i].logName} is active.");
            }

            for (int i = 0; i < playerOrderGame.Count; i++)
            {
                yield return playerOrderGame[i].TakeTurnRPC(playerOrderGame[i].photonplayer);
                yield return new WaitForSeconds(0.5f);
            }

            if (turnNumber+1 != 10)
            {
                sendingdata[0] = turnNumber + 1;
                PhotonNetwork.RaiseEvent(AdvanceTurnEvent, sendingdata, raiseEventOptions, SendOptions.SendReliable);
            }
            else
            {
                for (int i = 0; i<chosenEvents.Count; i++)
                    chosenEvents[i].choicescript.DisableButton();
                gameon = false;
                GameOver("The game has ended.", -1);
            }
        }
    }

    public void GameOver(string endText, int resignPosition)
    {
        Debug.Log($"{endText}, {resignPosition}");
        object[] sendingdata = new object[2];
        sendingdata[0] = endText;
        sendingdata[1] = resignPosition;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameOverEvent, sendingdata, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == AddNextPlayerEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int playerposition = (int)data[0];
            Photon.Realtime.Player playername = (Photon.Realtime.Player)data[1];

            playerOrderGame.Add(GameObject.Find(playername.NickName).GetComponent<Player>());
            playerOrderPhoton.Add(playername);

            PlayerButton nextButton = playerButtonClone[playerposition];
            nextButton.transform.localPosition = new Vector3(-1035, 425 - 125 * playerposition, 0);
            nextButton.name = $"{playername.NickName}'s Button";
            nextButton.transform.localScale = new Vector3(3, 3, 3);

            switch (playerposition)
            {
                case 0:
                    nextButton.image.color = Color.red;
                    break;
                case 1:
                    nextButton.image.color = Color.blue;
                    break;
                case 2:
                    nextButton.image.color = Color.yellow;
                    break;
                case 3:
                    nextButton.image.color = Color.white;
                    break;
            }

            playerOrderGame[playerposition].pv.RPC("AssignInfo", RpcTarget.All, playerposition, playername);
        }
        else if (photonEvent.Code == NextChosenEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int eventID = (int)data[0];
            Event nextEvent = PhotonView.Find(eventID).GetComponent<Event>();
            chosenEvents.Add(nextEvent);
            Log.instance.AddText($"Using {nextEvent.logName}.");
            nextEvent.transform.SetParent(eventBar);
        }
        else if (photonEvent.Code == AdvanceTurnEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int x = (int)data[0];
            turnNumber = x;
            turnText.text = $"{turnNumber+1}/10";

            for (int i = 0; i<chosenEvents.Count; i++)
            {
                if (chosenEvents[i].active[turnNumber])
                    chosenEvents[i].choicescript.EnableButton(null, true);
                else
                    chosenEvents[i].choicescript.DisableButton();
            }
        }
        else if (photonEvent.Code == GameOverEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            string endgame = (string)data[0];
            int resigningPlayer = (int)data[1];
            Player rp = null;

            endText.transform.parent.gameObject.SetActive(true);
            endText.transform.parent.SetAsLastSibling();
            Log.instance.transform.SetAsLastSibling();

            instructions.text = endgame;
            blownUp.SetActive(false);

            if (resigningPlayer > -1)
                rp = playerOrderGame[resigningPlayer];

            for (int i = 0; i < playerOrderGame.Count; i++)
                playerOrderGame[i].CalculateScore();

            playerOrderGame = playerOrderGame.OrderByDescending(o => o.score).ToList();
            endText.text = "";
            int playerTracker = 1;

            for (int i = 0; i < playerOrderGame.Count; i++)
            {
                if (i != resigningPlayer)
                {
                    endText.text += $"\n{playerTracker}: {playerOrderGame[i].name}, {playerOrderGame[i].score} POINTS";

                    if (i + 1 < playerOrderGame.Count && playerOrderGame[i].score > playerOrderGame[i + 1].score)
                        playerTracker++;
                }
            }
            if (rp != null)
                endText.text += $"\n\nResigned: {rp.name}: {rp.score} POINTS";
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void NewImage(Image image)
    {
        blownUp.transform.SetAsLastSibling();
        blownUp.SetActive(true);
        blownUp.transform.GetChild(0).GetComponent<Image>().sprite = image.sprite;
    }

    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("1. Lobby");
    }
}
