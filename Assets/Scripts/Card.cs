using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class Card : MonoBehaviour
{
    [HideInInspector] public string logName;
    [HideInInspector] public PhotonView pv;
    [HideInInspector] public Image image;
    [HideInInspector] public SendChoice choicescript;
    [HideInInspector] public bool director;
    [HideInInspector] public Vector3 originalPos;

    private void Awake()
    {
        originalPos = this.transform.localPosition;
        pv = this.GetComponent<PhotonView>();
        image = GetComponent<Image>();
        choicescript = GetComponent<SendChoice>();
    }

}
