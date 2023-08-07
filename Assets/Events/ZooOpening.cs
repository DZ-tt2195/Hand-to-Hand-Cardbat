using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZooOpening : Event
{
    public override void Setup()
    {
        logName = "Zoo Opening";
        thisTrigger = EventTrigger.Other;
        active[2 - 1] = true;
        active[3 - 1] = true;
        active[4 - 1] = true;
        active[8 - 1] = true;
        active[9 - 1] = true;
        active[10 - 1] = true;
    }

}
