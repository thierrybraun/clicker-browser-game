using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "BuildingCost/Linear")]
public class CostFunctionLinear : CostFunction
{
    public override Currency GetCost(int level)
    {
        if (Base == new Currency()) throw new Exception("Base is 0");
        return Base * level;
    }
}
