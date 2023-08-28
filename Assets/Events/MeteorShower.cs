using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeteorShower : Event
{
    public override void Setup()
    {
        logName = "Meteor Shower";
        thisTrigger = EventTrigger.Other;
        active[5 - 1] = true;
    }
}
