using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Collector : MonoBehaviour
{
    [HideInInspector] public Player user;
    [HideInInspector] public PhotonView pv;
    [HideInInspector] public TMP_Text textbox;
    RectTransform imageWidth;

    public SendChoice cardButton;
    public SendChoice textButton;
    public List<SendChoice> buttonsInCollector = new List<SendChoice>();

    void Awake()
    {
        pv = this.GetComponent<PhotonView>();
        imageWidth = this.transform.GetChild(0).GetComponent<RectTransform>();
        textbox = this.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    [PunRPC]
    public void StatsSetup(string text, int playerPosition, int thisPosition)
    {
        this.textbox.text = text;
        this.transform.SetParent( Manager.instance.canvas.transform);
        this.transform.localPosition = new Vector3(0, thisPosition, 0);
        this.user = Manager.instance.playerOrderGame[playerPosition];
    }

    [PunRPC]
    public void DestroyButton(int sibling)
    {
        SendChoice toDestroy = this.transform.GetChild(2).transform.GetChild(sibling).GetComponent<SendChoice>();
        buttonsInCollector.Remove(toDestroy);
        Destroy(toDestroy.gameObject);

        Debug.Log(this.transform.GetChild(2).transform.childCount);
        if (this.transform.GetChild(2).transform.childCount <= 1)
            PhotonNetwork.Destroy(this.pv);
    }

    [PunRPC]
    public void DestroyOtherButtons(int cardID)
    {
        while (buttonsInCollector.Count > 0)
        {
            SendChoice toDestroy = buttonsInCollector[0];
            buttonsInCollector.Remove(toDestroy);
            Destroy(toDestroy.gameObject);
        }

        AddCard(cardID, false);
    }

    [PunRPC]
    public void AddCard(int cardID, bool enabled)
    {
        SendChoice nextButton = Instantiate(cardButton, this.transform.GetChild(2));
        Card myCard = PhotonView.Find(cardID).GetComponent<Card>();
        nextButton.card = myCard;
        nextButton.image.sprite = myCard.image.sprite;

        PlayerCard differentCard = myCard.GetComponent<PlayerCard>();
        if (differentCard != null)
            nextButton.image.sprite = differentCard.originalImage;

        if (enabled && user.pv.AmOwner)
        {
            nextButton.EnableButton(user, enabled);
            nextButton.border.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 350);
            buttonsInCollector.Add(nextButton);
        }

        if (this.transform.GetChild(2).childCount <= 2)
            imageWidth.sizeDelta = new Vector2(500, 400);
        else
            imageWidth.sizeDelta = new Vector2(100 + 250 * (this.transform.GetChild(2).childCount), 400);
    }

    [PunRPC]
    public void AddText(string text, bool enabled)
    {
        SendChoice nextButton = Instantiate(textButton, this.transform.GetChild(2));
        nextButton.textbox.text = text;

        if (enabled && user.pv.AmOwner)
        {
            nextButton.EnableButton(user, enabled);
            buttonsInCollector.Add(nextButton);
        }

        if (this.transform.GetChild(2).childCount <= 2)
            imageWidth.sizeDelta = new Vector2(500, 240);
        else
            imageWidth.sizeDelta = new Vector2(500 * (this.transform.GetChild(2).childCount), 240);
    }

    public void DisableAll()
    {
        for (int i = 0; i<buttonsInCollector.Count; i++)
        {
            if (buttonsInCollector[i] != null)
                buttonsInCollector[i].DisableButton();
        }
    }

    public void EnableAll()
    {
        for (int i = 0; i<buttonsInCollector.Count; i++)
        {
            if (buttonsInCollector[i] != null)
                buttonsInCollector[i].EnableButton(user, true);
        }
    }

}
