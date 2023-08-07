using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Masquerade : Event
{
    public override void Setup()
    {
        logName = "Masquerade";
        thisTrigger = EventTrigger.Other;
        active[3 - 1] = true;
        active[6 - 1] = true;
        active[9 - 1] = true;
    }
}
