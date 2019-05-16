using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Data/Resource", order = 1)]
public class Resource : ScriptableObject
{
    public API.ResourceType ApiType;
    public GameObject Prefab;
    public Building Building;
}
