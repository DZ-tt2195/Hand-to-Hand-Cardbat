using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Vacation : Event
{
    public override void Setup()
    {
        logName = "Vacation";
        thisTrigger = EventTrigger.Other;
        active[6 - 1] = true;
        active[7 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
        active[10 - 1] = true;
    }
}
