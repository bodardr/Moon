using System.Collections.Generic;
using UnityEngine;

public class CraftingBuilding : BuildingBaseSingleton<CraftingBuilding>
{
    public List<CraftingRecipe> Recipes { get; private set; } = new();

    private CraftingRecipe currentRecipe;
    private GearTarget gearTarget;
    private float recipeTime;
    private bool craftingActive;

    public float CraftingRatio => Mathf.Clamp01(1 - recipeTime / (currentRecipe?.CraftingTime ?? 1));
    
    protected override void InitializeUpgrades()
    {
        
    }

    private void Update()
    {
        if (!craftingActive)
            return;
        
        recipeTime -= Time.deltaTime * gearTarget.Stats.SpeedRate.EvaluatedValue;
        if (recipeTime <= 0)
            CompleteCrafting();
    }

    private void CompleteCrafting()
    {
        recipeTime = 0;
        currentRecipe.UpdateUnlockCondition();
        currentRecipe = null;
    }
}