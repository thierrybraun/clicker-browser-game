using UnityEngine;
using Model;
using System.Linq;

public class CityLoader : MonoBehaviour
{
    public static int TILE_SIZE = 10;
    public GameObject Flat, Water, Hills;
    public GameObject House, Applefarm, Lumberjack, Fishingboat, Mine;
    public GameObject Apples, Fish, Forest, Ore;
    private GameObject terrain;

    private void Start()
    {
        terrain = new GameObject("Terrain");
        terrain.transform.SetParent(transform);
    }

    public void LoadCity(City map)
    {
        Destroy(terrain);
        terrain = new GameObject("Terrain");
        terrain.transform.SetParent(transform);

        var rand = new System.Random(0);
        for (int i = 0; i < map.height; i++)
        {
            for (int j = 0; j < map.width; j++)
            {
                var field = map.fields.First(f => f.x == j && f.y == i);
                var tile = Instantiate<GameObject>(GetTile(field.fieldType));
                tile.transform.SetParent(terrain.transform);
                tile.transform.position = new Vector3(j * TILE_SIZE, 0, i * TILE_SIZE);
                var comp = tile.AddComponent<Tile>();
                comp.Field = field.fieldType;
                comp.Resource = field.resourceType;
                comp.Building = field.buildingType;
                comp.X = j;
                comp.Y = i;
                var collider = tile.AddComponent<BoxCollider>();
                collider.size = new Vector3(TILE_SIZE, TILE_SIZE, TILE_SIZE);
                collider.center = new Vector3(0, -TILE_SIZE / 2, 0);
                var rot = rand.Next(4);
                tile.transform.GetChild(0).Rotate(new Vector3(0, rot * 90, 0), Space.World);
                tile.name = "Tile_" + j + "_" + i;

                var buildingType = field.buildingType;
                if (buildingType.HasValue)
                {
                    var model = Instantiate(GetBuilding(buildingType.Value));
                    model.transform.SetParent(tile.transform, false);
                }
                else
                {
                    var resourceType = field.resourceType;
                    if (resourceType.HasValue)
                    {
                        var resource = Instantiate<GameObject>(GetResource(resourceType.Value));
                        resource.transform.SetParent(tile.transform, false);
                    }
                }
            }
        }

    }

    private GameObject GetTile(FieldType type)
    {
        switch (type)
        {
            case FieldType.Plain:
                return Flat;
            case FieldType.Water:
                return Water;
            case FieldType.Hills:
                return Hills;
        }
        throw new System.Exception("No type defined: " + type);
    }

    private GameObject GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Apples:
                return Apples;
            case ResourceType.Fish:
                return Fish;
            case ResourceType.Forest:
                return Forest;
            case ResourceType.Ore:
                return Ore;
        }
        throw new System.Exception("No type defined: " + type);
    }

    private GameObject GetBuilding(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Applefarm:
                return Applefarm;
            case BuildingType.Fishingboat:
                return Fishingboat;
            case BuildingType.House:
                return House;
            case BuildingType.Lumberjack:
                return Lumberjack;
            case BuildingType.Mine:
                return Mine;
        }
        throw new System.Exception("No type defined: " + type);
    }
}
