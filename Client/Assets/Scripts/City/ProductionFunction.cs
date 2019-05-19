using UnityEngine;

public abstract class ProductionFunction : ScriptableObject
{
    public int FoodBase, WoodBase, MetalBase;

    public abstract Production GetProduction(int level);

    public struct Production
    {
        public int Food, Wood, Metal;
    }
}
