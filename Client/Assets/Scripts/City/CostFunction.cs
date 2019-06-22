using System;
using UnityEngine;

public abstract class CostFunction : ScriptableObject
{
    public Currency Base;

    public abstract Currency GetCost(int level);

    public Currency GetCostRange(int currentLevel, int targetLevel)
    {
        var current = new Currency();
        for (int i = currentLevel + 1; i <= targetLevel; i++)
        {
            current += GetCost(i);
        }

        return current;
    }

    public int GetMaxLevel(int currentLevel, Currency available)
    {
        if (available == new Currency()) throw new Exception("Currency is 0");
        int targetLevel = currentLevel;
        var cost = GetCost(targetLevel + 1);

        while (cost <= available)
        {
            targetLevel++;
            cost += GetCost(targetLevel + 1);
        }

        return targetLevel;
    }
}
