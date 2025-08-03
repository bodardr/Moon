using System;
using System.Reflection;
namespace Loadout.Editor
{
    public abstract class LoadoutElementBase : ILoadoutElement
    {
        public string SaveKey { get; set; }
        public object Value { get; set; }
        public bool Active { get; set; } = false;

        protected bool isType = false;
        
        public LoadoutElementBase(FieldInfo fieldInfo)
        {
            SaveKey = $"{fieldInfo.DeclaringType.FullName}::{fieldInfo.Name}";
            Value = Activator.CreateInstance(fieldInfo.FieldType);
        }

        public LoadoutElementBase(Type type)
        {
            isType = true;
            SaveKey = $"OBJECT-{type.FullName}";
            Value = Activator.CreateInstance(type);
        }
        
        public abstract void Load(LoadoutSave loadoutSave);
        public abstract void Save(LoadoutSave loadoutSave);
    }
}