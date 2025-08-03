using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loadout.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Loadout
{
    public static class LoadoutLoader
    {
        private const string LoadoutConsolePrefix = "[<b>Loadout</b>]";

        private static Dictionary<Type, object> current;

        public static List<Type> GetAllTypes() => TypeCache.GetTypesWithAttribute<LoadoutAttribute>().ToList();
        public static List<FieldInfo> GetAllFields() => TypeCache.GetFieldsWithAttribute<LoadoutAttribute>().ToList();

        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCurrentLoadout()
        {
            current = new();

            var fields = GetAllFields();
            LoadTypes();

            var loadoutSave = GetLoadoutSave();

            if (loadoutSave != null)
                InitializeLoadoutKeys(loadoutSave, fields);
        }
        private static void LoadTypes()
        {

        }

        private static void InitializeLoadoutKeys(LoadoutSave loadoutSave, List<FieldInfo> fields)
        {
            foreach (var field in fields)
            {
                if (!field.IsStatic)
                {
                    Debug.LogWarning(
                        $"{LoadoutConsolePrefix} Cannot assign value to non-static field <b>{field.DeclaringType}</b> <b>{field.Name}</b>. Make it static.");
                    continue;
                }

                var attribute = (LoadoutAttribute)field.GetCustomAttribute(typeof(LoadoutAttribute));

                ILoadoutElement loadoutElement;
                switch (attribute.storageType)
                {
                    case LoadoutAttribute.StorageType.File:
                        loadoutElement = new LoadoutElementFile(field);
                        break;
                    default:
                    case LoadoutAttribute.StorageType.EditorPrefs:
                        loadoutElement = new LoadoutKeyPrefs(field);
                        break;
                }
                loadoutElement.Load(loadoutSave);

                if (!loadoutElement.Active)
                    continue;

                //Injects the static value.
                field.SetValue(null, loadoutElement.Value);
            }
        }
        public static LoadoutSave GetLoadoutSave()
        {
            const string keysAssetPath = "Assets/Loadout/LoadoutKeys.asset";
            if (!AssetDatabase.AssetPathExists(keysAssetPath))
                return new LoadoutSave(new());

            var loadoutKeys = new LoadoutKeys();
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(keysAssetPath);
            if (textAsset == null)
                return new LoadoutSave(new());

            loadoutKeys = JsonConvert.DeserializeObject<LoadoutKeys>(textAsset.text);
            return new LoadoutSave(loadoutKeys);
        }

        public static T GetValue<T>()
        {
           var val = current.GetValueOrDefault(typeof(T));
           if(val != null)
               return (T)val;
           return default;
        }
    }
}
