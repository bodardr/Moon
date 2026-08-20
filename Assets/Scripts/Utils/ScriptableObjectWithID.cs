using Sirenix.OdinInspector;
using UnityEngine;
public class ScriptableObjectWithID : ScriptableObject
{
    [ReadOnly] [SerializeField] private string uid;

    public string UID => uid;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uid))
            uid = System.Guid.NewGuid().ToString();
    }
}
