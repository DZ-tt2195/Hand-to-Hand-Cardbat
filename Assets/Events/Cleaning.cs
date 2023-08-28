using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cleaning : Event
{
    public override void Setup()
    {
        logName = "Cleaning";
        thisTrigger = EventTrigger.Other;
        active[2 - 1] = true;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[5 - 1] = true;
        active[6 - 1] = true;
        active[7 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
    }

}
