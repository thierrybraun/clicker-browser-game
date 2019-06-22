using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "BuildingCost/Linear")]
public class CostFunctionLinear : CostFunction
{
    public override Currency GetCost(int level)
    {
        return Base * level;
    }
}
