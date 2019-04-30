using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public FieldType Field;
    public ResourceType? Resource;
    public BuildingType? Building;

    private void OnMouseUpAsButton()
    {        
        BuildingType? building = null;
        switch (Resource)
        {
            case ResourceType.Apples:
                building = BuildingType.Applefarm;
                break;
            case ResourceType.Fish:
                building = BuildingType.Fishingboat;
                break;
            case ResourceType.Forest:
                building = BuildingType.Lumberjack;
                break;
            case ResourceType.Ore:
                building = BuildingType.Mine;
                break;
        }

        if (building.HasValue)
        {
            FindObjectOfType<GameController>().Build(building.Value, X, Y);
        }
    }
}
