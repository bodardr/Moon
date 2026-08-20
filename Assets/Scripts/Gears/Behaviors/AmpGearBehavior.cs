using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AmpGearBehavior : IGearBehavior
{
    [SerializeField] private uint addedAmplitude;
    
    public void OnGearTriggered(GearTarget[] targets, ref uint amplitude, List<Gear> gearColumn,
        Dictionary<string, object> additionalParameters)
    {
        amplitude += addedAmplitude;
    }
}
