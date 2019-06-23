using UnityEngine;

public abstract class ProductionFunction : ScriptableObject
{
    public Currency Base;

    public abstract Currency GetProduction(int level);
}
