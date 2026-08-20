using System;
using UnityEngine;

[Serializable]
public class CraftingRecipe : IGearUnlockCondition
{
    private const float CRAFTING_TIME_PER_TIER = 25f;

    [SerializeField] private Gear gearA;
    [SerializeField] private Gear gearB;

    [SerializeField] private uint craftingTier = 1;

    public float CraftingTime => craftingTier * CRAFTING_TIME_PER_TIER;
    public event Action OnUnlocked;

    public void Subscribe()
    {
        CraftingBuilding.Instance.Recipes.Add(this);
    }
    public void Unsubscribe()
    {
        CraftingBuilding.Instance.Recipes.Remove(this);
    }
    public void UpdateUnlockCondition()
    {
        OnUnlocked?.Invoke();
    }
}