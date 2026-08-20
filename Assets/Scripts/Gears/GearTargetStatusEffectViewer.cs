using Bodardr.Databinding.Runtime;
using UnityEngine;
public class GearTargetStatusEffectViewer : MonoBehaviour
{
    [SerializeField] private GearTarget gearTarget;
    
    private BindingCollectionBehavior collectionBehavior;

    private void Awake()
    {
        collectionBehavior = GetComponent<BindingCollectionBehavior>();
    }

    private void Update()
    {
        collectionBehavior.Collection = gearTarget.StatusEffects.Values;
    }
}
