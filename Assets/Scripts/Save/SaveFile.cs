using System;
using System.Collections.Generic;
using System.IO;
using Loadout;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
namespace Save
{
    [Loadout]
    [Serializable]
    public class SaveFile
    {
        public float dayTime;
        public float normalizedMoonTime;
        
        public uint moonDamage;
        
        public uint energy;
        public uint wood;
        public uint stone;
        public uint lunarite;
        public uint iron;
        public uint gold;
        public uint moonDamageTier;
        
        public HashSet<string> buildingUpgrades = new();

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
            Current = loadoutSave ?? JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(FilePath)) ?? new SaveFile();
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
