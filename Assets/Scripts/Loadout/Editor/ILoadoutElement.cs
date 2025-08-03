namespace Loadout.Editor
{
    public interface ILoadoutElement
    {
        public string SaveKey { get; set; }
        public object Value { get; set; }
        public bool Active { get; set; }

        public void Load(LoadoutSave loadoutSave);
        
        public void Save(LoadoutSave loadoutSave);
    }
}
