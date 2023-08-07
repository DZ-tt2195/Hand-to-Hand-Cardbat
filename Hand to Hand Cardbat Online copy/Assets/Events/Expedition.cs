using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Expedition : Event
{
    public override void Setup()
    {
        logName = "Expedition";
        thisTrigger = EventTrigger.Other;
        active[1 - 1] = true;
        active[2 - 1] = true;
    }
}
