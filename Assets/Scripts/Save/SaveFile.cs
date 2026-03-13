using System;
using System.Collections.Generic;
using System.IO;
using Loadout;
using Newtonsoft.Json;
using UnityEngine;

namespace Save
{
    [Loadout]
    [Serializable]
    public class SaveFile
    {
        public float dayTime;
        public float normalizedMoonTime;
        
        public uint moonDamage;
        public ResourceWithAmount Lux = new(ResourceType.Lux);
        
        public uint moonDamageTier;
        
        public HashSet<string> buildingUpgrades = new();

        public ResourceWithAmount this[ResourceType resourceType] => resourceType switch
        {
            ResourceType.Lux => Lux
        };
        
        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
        public static SaveFile Current { get; private set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init()
        {
            Application.quitting += Save;
            Load();
        }
        
        private static void Load()
        {
            var loadoutSave = LoadoutLoader.GetValue<SaveFile>();
            Current = loadoutSave ?? (File.Exists(FilePath) ? JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(FilePath)) : new SaveFile());
        }

        public static void Save()
        {
            Application.quitting -= Save;
            
            //If loadout is active don't save.
            if (LoadoutLoader.GetValue<SaveFile>() != null)
                return;
            
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current));
        }
    }
}
