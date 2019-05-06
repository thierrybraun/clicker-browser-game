using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Data/Building", order = 1)]
public class Building : ScriptableObject
{
    public Model.BuildingType ApiType;
    public GameObject Prefab;
}
