using System;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;

namespace Loadout.Editor
{
    public class LoadoutKeyPrefs : LoadoutElementBase
    {
        public LoadoutKeyPrefs(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }

        public LoadoutKeyPrefs(Type type) : base(type)
        {
        }
        
        public override void Load(LoadoutSave loadoutSave)
        {
            var keyAddress = $"Loadout/{SaveKey}";

            if (EditorPrefs.HasKey(keyAddress))
            {
                try
                {
                    Value = JsonConvert.DeserializeObject(EditorPrefs.GetString(keyAddress), Value.GetType());
                }
                catch(Exception)
                {
                    // ignored
                }
            }

            Active = loadoutSave.activePrefKeys.Contains(SaveKey);
        }

        public override void Save(LoadoutSave loadoutSave)
        {
            EditorPrefs.SetString($"Loadout/{SaveKey}", JsonConvert.SerializeObject(Value));

            if (Active)
                loadoutSave.activePrefKeys.Add(SaveKey);
            else
                loadoutSave.activePrefKeys.Remove(SaveKey);
        }
    }
}
