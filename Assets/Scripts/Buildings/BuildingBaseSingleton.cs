using UnityEngine;
public abstract class BuildingBaseSingleton<T> : BuildingBase where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = GetInstance();

            return instance;
        }
        private set => instance = value;
    }

    public virtual void DestroyCallback()
    {
        if (!instance)
            instance = null;
    }

    private static T GetInstance()
    {
        return FindAnyObjectByType<T>(FindObjectsInactive.Include);
    }
}
