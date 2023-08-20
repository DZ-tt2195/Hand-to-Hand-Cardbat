using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerCard : Card
{
    [HideInInspector] public enum CardColor { Blue, Gold, Red, None };
    [HideInInspector] public CardColor myColor;
    [HideInInspector] public int myCost;
    [HideInInspector] public int myCrowns;
    [HideInInspector] public bool rotated;
    [HideInInspector] public int suitCode;
    public Sprite originalImage;

    void Start()
    {
        Setup();

        switch (myColor)
        {
            case CardColor.Blue:
                suitCode = 0 + myCost;
                break;
            case CardColor.Gold:
                suitCode = 100 + myCost;
                break;
            case CardColor.Red:
                suitCode = 200 + myCost;
                break;
        }
    }

    public virtual void Setup()
    {
    }

    public IEnumerator InitialCommand(Player currPlayer, Card source)
    {
        Log.instance.pv.RPC("AddText", RpcTarget.All, $"");

        if (Manager.instance.turnNumber == 0)
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{currPlayer.name} fails to command {this.logName}.");
            yield break;
        }

        if (Manager.instance.EventActive("Zoo Opening"))
        {
            yield return currPlayer.ChooseToPlay(currPlayer.listOfHand, "Zoo Opening");
            if (currPlayer.choice != "Decline")
                yield break;
        }

        if (source.director && this.director)
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{currPlayer.name} fails to command {this.logName}.");
            yield break;
        }
        else
        {
            Log.instance.pv.RPC("AddText", RpcTarget.All, $"{currPlayer.name} commands {this.logName}.");
            yield return this.NowCommand(currPlayer);
        }
    }

    public virtual IEnumerator NowCommand(Player currPlayer)
    {
        yield return null;
    }

    [PunRPC]
    public void RotateMe()
    {
        rotated = true;
        transform.localEulerAngles = new Vector3(0, 0, 90);
        image.color = new Color(0.5f, 0.5f, 0.5f);
    }

    [PunRPC]
    public void UnRotateMe()
    {
        rotated = false;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        image.color = new Color(1, 1, 1);
    }

    public virtual IEnumerator PlayEffect(Player currPlayer)
    {
        yield return null;
    }

    public virtual IEnumerator DiscardEffect(Player currPlayer, int code)
    {
        yield return null;
        currPlayer.pv.RPC("PutInDiscard", RpcTarget.All, this.pv.ViewID, code);
    }

    public bool CanPlayThis(Player currPlayer)
    {
        bool haveMoney = currPlayer.coins >= this.myCost;
        bool satisfiesLockdown = false;

        if (Manager.instance.EventActive("Lockdown"))
        {
            for (int i = 0; i<currPlayer.listOfPlay.Count; i++)
            {
                PlayerCard nextCard = currPlayer.listOfPlay[i];
                if (nextCard.myColor == this.myColor && nextCard.myCost == this.myCost)
                {
                    satisfiesLockdown = true;
                    break;
                }
            }
        }
        else
        {
            satisfiesLockdown = true;
        }

        return haveMoney && satisfiesLockdown;
    }
}
