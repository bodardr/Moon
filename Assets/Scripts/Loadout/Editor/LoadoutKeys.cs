using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
[Serializable]
public class LoadoutKeys : ISerializationCallbackReceiver
{
    [NonSerialized]
    private Dictionary<string, string> loadoutKeys = new();
    
    [NonSerialized]
    private Dictionary<string, string> loadoutTypes = new();
    
    [NonSerialized]
    private HashSet<string> enabledKeysSet = new();
    
    [NonSerialized]
    private HashSet<string> enabledTypesSet = new();
    
    [SerializeField]
    private string[] enabledKeys;
    
    [SerializeField]
    private string[] enabledTypes;
    
    [FormerlySerializedAs("keys")]
    [SerializeField]
    private string[] keysDictKeys;
    
    [FormerlySerializedAs("values")]
    [SerializeField]
    private string[] keysDictValues;

    [SerializeField]
    private string[] typesDictKeys;

    [SerializeField]
    private string[] typesDictValues;
    
    public Dictionary<string, string> LoadoutKeysDict => loadoutKeys;
    public Dictionary<string, string> LoadoutTypesDict => loadoutTypes;
    public HashSet<string> EnabledKeys => enabledKeysSet;
    public HashSet<string> EnabledTypes => enabledTypesSet;

    public void OnBeforeSerialize()
    {
        loadoutKeys.Serialize(out keysDictKeys, out keysDictValues);
        loadoutTypes.Serialize(out typesDictKeys, out typesDictValues);
    }

    public void OnAfterDeserialize()
    {
        loadoutKeys = DictionaryExtensions.Deserialize<string, string>(keysDictKeys, keysDictValues);
        loadoutTypes = DictionaryExtensions.Deserialize<string, string>(typesDictKeys, typesDictValues);
        enabledKeysSet = enabledKeys.ToHashSet();
        enabledTypesSet = enabledTypes.ToHashSet();
    }
}
