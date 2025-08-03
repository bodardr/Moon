using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Loadout.Editor
{
    public class LoadoutSave
    {
        public HashSet<string> activePrefKeys = new();
        public LoadoutKeys loadoutKeys;

        public LoadoutSave(LoadoutKeys loadoutKeys)
        {
            this.loadoutKeys = loadoutKeys;

            try
            {
                var activePrefsArray =
                    JsonConvert.DeserializeObject<string[]>(EditorPrefs.GetString("Loadout-ActivePrefs"));

                if (activePrefsArray != null)
                    activePrefKeys = activePrefsArray?.ToHashSet();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Save()
        {
            var activeEditorPrefs = activePrefKeys.ToArray();
            EditorPrefs.SetString("Loadout-ActivePrefs", JsonConvert.SerializeObject(activeEditorPrefs));

            CreateFolderIfNotExists("Assets/Loadout");
            AssetDatabase.CreateAsset(new TextAsset(JsonConvert.SerializeObject(loadoutKeys)),
                "Assets/Loadout/LoadoutKeys.asset");
            AssetDatabase.SaveAssets();
        }

        private static void CreateFolderIfNotExists(string folderPath)
        {
            var slashCounter = 0;
            for (var i = 1; i < folderPath.Length; i++)
            {
                if (i < folderPath.Length - 1 && folderPath[i] is not ('/' or '\\'))
                    continue;

                if (++slashCounter < 2)
                    continue;

                var path = folderPath[..(folderPath[i] is not ('/' or '\\') ? i + 1 : i)];
                if (AssetDatabase.IsValidFolder(path))
                    continue;

                var lastSlash = path.LastIndexOf('/');
                AssetDatabase.CreateFolder(path[..lastSlash], path[(lastSlash + 1)..]);
            }
        }
    }
}