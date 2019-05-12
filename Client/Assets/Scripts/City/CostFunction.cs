using UnityEngine;

public abstract class CostFunction : ScriptableObject
{
    public int FoodBase, WoodBase, MetalBase;

    public abstract BuildingCost GetCost(int level);

    public struct BuildingCost
    {
        public int Food, Wood, Metal;
    }
}
