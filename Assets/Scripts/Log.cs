using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class Log : MonoBehaviour
{
    public static Log instance;
    [HideInInspector] public PhotonView pv;
    [HideInInspector] TMP_Text textBox;
    [HideInInspector] RectTransform textRT;
    [HideInInspector] Scrollbar scroll;
    int linesOfText = 0;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        textBox = this.transform.GetChild(0).GetComponent<TMP_Text>();
        textRT = textBox.GetComponent<RectTransform>();
        scroll = this.transform.GetChild(1).GetComponent<Scrollbar>();
        instance = this;
        textBox.text = "";
    }

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
                AddText($"Test {linesOfText}");
        #endif
    }

    [PunRPC]
    public void AddText(string text)
    {
        linesOfText++;
        textBox.text += text + "\n";

        if (linesOfText >= 40)
        {
            textRT.sizeDelta = new Vector2(510, textRT.sizeDelta.y+34.5f);

            if (scroll.value <= 0.2f)
            {
                textRT.localPosition = new Vector2(-40, textRT.localPosition.y + 25);
                scroll.value = 0;
            }
        }
    }
}
