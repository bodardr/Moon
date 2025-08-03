using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Loadout.Editor
{
    public class LoadoutElementFile : LoadoutElementBase
    {
        public LoadoutElementFile(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }

        public LoadoutElementFile(Type type) : base(type)
        {
        }

        public override void Load(LoadoutSave loadoutSave)
        {
            if (loadoutSave.loadoutKeys == null)
                return;
            
            var enabledSet = isType ? loadoutSave.loadoutKeys.EnabledTypes : loadoutSave.loadoutKeys.EnabledKeys;
            var dict = isType ? loadoutSave.loadoutKeys.LoadoutTypesDict : loadoutSave.loadoutKeys.LoadoutKeysDict;

            Active = enabledSet.Contains(SaveKey);
            
            if (!dict.TryGetValue(SaveKey, out var loadoutJson))
                return;

            try
            {
                Value = JsonConvert.DeserializeObject(loadoutJson, Value.GetType());
            }
            catch(Exception)
            {
                // ignored
            }
        }

        public override void Save(LoadoutSave loadoutSave)
        {
            var enabledSet = isType ? loadoutSave.loadoutKeys.EnabledTypes : loadoutSave.loadoutKeys.EnabledKeys;
            var dict = isType ? loadoutSave.loadoutKeys.LoadoutTypesDict : loadoutSave.loadoutKeys.LoadoutKeysDict;
            
            dict[SaveKey] = JsonConvert.SerializeObject(Value);

            if (Active)
                enabledSet.Add(SaveKey);
            else
                enabledSet.Remove(SaveKey);
        }
    }
}
