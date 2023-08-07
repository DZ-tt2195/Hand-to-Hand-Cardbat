using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.EventSystems;

public class PlayerButton : MonoBehaviour
{
    [HideInInspector] public Image image;
    [HideInInspector] public Button button;
    [HideInInspector] public PhotonView pv;

    public TMP_Text myName;
    public TMP_Text myCards;
    public TMP_Text myCoins;
    public TMP_Text myCrowns;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        pv = GetComponent<PhotonView>();
    }

}
