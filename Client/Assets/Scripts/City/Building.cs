using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Data/Building", order = 1)]
public class Building : ScriptableObject
{
    public API.BuildingType ApiType;
    public GameObject Prefab;
    public Sprite CollectionSprite;
    public CostFunction BuildCostFunction;
    public ProductionFunction ProductionFunction;
}
