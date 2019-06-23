using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Production/Linear")]
public class ProductionFunctionLinear : ProductionFunction
{
    public override Currency GetProduction(int level)
    {
        return Base * level;
    }
}
