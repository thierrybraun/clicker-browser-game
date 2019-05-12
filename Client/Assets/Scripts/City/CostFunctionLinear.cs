using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "BuildingCost/Linear")]
public class CostFunctionLinear : CostFunction
{
    public override BuildingCost GetCost(int level)
    {
        return new BuildingCost
        {
            Food = FoodBase * level,
            Wood = WoodBase * level,
            Metal = MetalBase * level
        };
    }
}
