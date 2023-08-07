using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SendChoice : MonoBehaviour
{
    [HideInInspector] public Player choosingplayer;
    Button button;
    [HideInInspector] public Image image;

    public TMP_Text textbox;
    [HideInInspector] public Image border;
    [HideInInspector] bool enableBorder;
    [HideInInspector] public Card card;

    void Awake()
    {
        button = this.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(SendName);
        border = this.transform.GetChild(0).GetComponent<Image>();
        card = this.GetComponent<Card>();
    }

    private void FixedUpdate()
    {
        if (border != null && enableBorder)
        {
            border.color = new Color(1, 1, 1, Manager.instance.opacity);
        }
        else if (border != null && !enableBorder)
        {
            border.color = new Color(1, 1, 1, 0);
        }
    }

    public void EnableButton(Player player, bool border)
    {
        this.gameObject.SetActive(true);
        choosingplayer = player;
        enableBorder = border;
        if (button != null)
            button.interactable = true;
    }

    public void DisableButton()
    {
        if (button != null)
            button.interactable = false;
        enableBorder = false;
    }

    public void SendName()
    {
        if (textbox != null)
            choosingplayer.choice = textbox.text;
        else
            choosingplayer.choice = this.name;

        choosingplayer.chosencard = card;
        choosingplayer.chosenbutton = this;
    }

}
