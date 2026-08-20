using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

public class GearSaveHandler : MonoSingleton<GearSaveHandler>, ILoadCallback, ISaveCallback
{
    private Dictionary<string, GearHolder> gearHolders;

    public void OnLoad(SaveFile saveFile)
    {
        gearHolders =
            ((GearHolder[])FindObjectsByType(typeof(GearHolder), FindObjectsInactive.Include)).ToDictionary(x => x.name, x => x);
        
        var savedSequences = saveFile.savedGearSequences;
        foreach (var (key, sequenceRaw) in savedSequences)
        {
            var gearSequence =
                sequenceRaw.ConvertAll(x => x.ConvertAll(y => y.ConvertAll(z => Gear.AllGears[z])));
            gearHolders[key].Gears = gearSequence;
        }
    }

    public void OnSave(SaveFile saveFile)
    {
        var savedSequences = saveFile.savedGearSequences;
        savedSequences.Clear();

        foreach (var (key, gearHolder) in gearHolders)
        {
            if (gearHolder.Gears == null)
                continue;
            
            savedSequences.Add(key,
                gearHolder.Gears.ConvertAll(x => x.ConvertAll(y => y.ConvertAll(z => z.UID))));
        }
    }
}
