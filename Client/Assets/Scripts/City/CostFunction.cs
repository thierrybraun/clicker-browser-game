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

    public int GetMaxLevel(int currentLevel, int availableFood, int availableWood, int availableMetal)
    {
        int targetLevel = currentLevel;
        var cost = GetCost(targetLevel + 1);

        while (cost.Food <= availableFood && cost.Wood <= availableWood && cost.Metal <= availableMetal)
        {
            cost += GetCost(targetLevel);
            targetLevel++;
        }

        return targetLevel;
    }
}
