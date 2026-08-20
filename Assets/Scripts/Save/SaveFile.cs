using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loadout;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Save
{
    [Serializable]
    public class SaveFile
    {
        [Loadout]
        private static bool forceCreateNewSave = false;
        private static Object[] allMonos;

        [NonSerialized] private List<ISaveCallback> saveCallbacks;

        public float dayTime;
        public float normalizedMoonTime;

        public uint moonDamage;
        public ResourceWithAmount Lux = new(ResourceType.Lux);

        public uint moonDamageTier;

        public HashSet<string> buildingUpgrades = new();
        public Dictionary<string, uint> availableGearInventory = new();
        public HashSet<string> gearUnlocks = new();
        public Dictionary<string, List<List<List<string>>>> savedGearSequences = new();

        public bool GearsUnlocked { get; set; } = false;

        public ResourceWithAmount this[ResourceType resourceType] => resourceType switch
        {
            ResourceType.Lux or _ => Lux
        };

        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
        public static SaveFile Current { get; private set; }

        [MenuItem("Save/Delete")]
        private static void DeleteSave()
        {
            File.Delete(FilePath);
            Current = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Awake()
        {
            Application.quitting += SaveOnQuit;
            Load();

            var currentGearUnlocks = Current.gearUnlocks;
            foreach (var unlock in GearUnlock.AllUnlocks)
            {
                if (currentGearUnlocks.Contains(unlock.UID))
                    continue;

                unlock.Initialize();
            }
        }

        private static void Load()
        {
            var createdNewSave = false;
            if (forceCreateNewSave)
            {
                Current = new SaveFile();
                createdNewSave = true;
            }
            else if (Current == null)
            {
                if (File.Exists(FilePath))
                {
                    Current = JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(FilePath));
                }
                else
                {
                    Current = new SaveFile();
                    createdNewSave = true;
                }
            }

            allMonos = Object.FindObjectsByType(typeof(MonoBehaviour), FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            if (createdNewSave)
            {
                var firstLoadCallbacks = allMonos.OfType<IFirstLoadCallback>().ToArray();

                foreach (var callback in firstLoadCallbacks)
                    callback.OnFirstLoad(Current);
            }

            var loadCallbacks = allMonos.OfType<ILoadCallback>().ToArray();

            foreach (var callback in loadCallbacks)
                callback.OnLoad(Current);
        }

        public static void SaveOnQuit()
        {
            Application.quitting -= SaveOnQuit;
            Save();
        }

        public static void Save()
        {
            //If loadout is active don't save.
            if (LoadoutLoader.GetValue<SaveFile>() != null)
                return;

            var saveCallbacks = allMonos.OfType<ISaveCallback>();

            foreach (var saveCallback in saveCallbacks)
                saveCallback.OnSave(Current);

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current, Formatting.Indented));
        }

        public void AddGearToInventory(Gear gearToAdd)
        {
            if (!availableGearInventory.TryAdd(gearToAdd.UID, 1))
                availableGearInventory[gearToAdd.UID]++;
        }
        
        public void RemoveGear(Gear gear)
        {
            if (!availableGearInventory.ContainsKey(gear.UID))
                return;
            
            availableGearInventory[gear.UID]--;
            if(availableGearInventory[gear.UID] <= 0)
                availableGearInventory.Remove(gear.UID);
        }
    }
}
