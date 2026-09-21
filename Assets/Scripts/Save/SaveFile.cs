using System;
using System.Collections.Generic;
using System.IO;
using Loadout;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Save
{
    [Serializable]
    public class SaveFile
    {
        [Loadout]
        private static bool forceCreateNewSave = false;

        public Currencies Currencies = new();

        public HashSet<string> upgrades = new();
        public Dictionary<string, int> tieredUpgrades = new();

        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
        public static SaveFile Current
        {
            get;
            private set;
        }

        #if UNITY_EDITOR
        [MenuItem("Save/Delete")]
        #endif
        private static void DeleteSave()
        {
            File.Delete(FilePath);
            Current = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Awake()
        {
            Application.quitting += SaveOnQuit;
            Load();
        }

        private static void Load()
        {

            if (forceCreateNewSave)
            {
                Current = new SaveFile();
            }
            else if (Current == null)
            {
                Current = File.Exists(FilePath) ? JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText(FilePath))
                    : new SaveFile();
            }

            Upgrades.LoadUpgrades();
        }

        public static void SaveOnQuit()
        {
            Application.quitting -= SaveOnQuit;
            Save();
        }

        public static void Save()
        {
            //If loadout is active don't save.
            if (LoadoutLoader.GetValue<MoonSaveFile>() != null)
                return;

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current, Formatting.Indented));
        }
    }
}
