using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class JobFair : Event
{
    public override void Setup()
    {
        logName = "Job Fair";
        thisTrigger = EventTrigger.Other;
        active[1 - 1] = true;
        active[3 - 1] = true;
        active[5 - 1] = true;
        active[7 - 1] = true;
        active[9 - 1] = true;
    }

}
