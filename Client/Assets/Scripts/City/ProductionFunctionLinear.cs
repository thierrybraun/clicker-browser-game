using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Production/Linear")]
public class ProductionFunctionLinear : ProductionFunction
{
    public override Production GetProduction(int level)
    {
        return new Production
        {
            Food = FoodBase * level,
            Wood = WoodBase * level,
            Metal = MetalBase * level
        };
    }
}
