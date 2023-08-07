using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Event : Card
{
    [HideInInspector]public bool[] active = new bool[10];
    public enum EventTrigger { TurnStart, TurnEnd, Other};
    [HideInInspector] public EventTrigger thisTrigger;

    private void Start()
    {
        Setup();
    }

    public virtual void Setup()
    {
    }

    public virtual IEnumerator UseEvent(Player currPlayer)
    {
        yield return null;
    }

    public bool IsActive(int turnNumber)
    {
        return active[turnNumber];
    }
}
