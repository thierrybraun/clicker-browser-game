using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CityLoader : MonoBehaviour
{
    public static int TILE_SIZE = 10;
    public GameObject Flat, Water, Hills;
    private GameObject terrain;

    private IDictionary<Vector2Int, Tile> tiles;

    private void Start()
    {
        terrain = new GameObject("Terrain");
        terrain.transform.SetParent(transform);
    }

    public Tile[] GetTiles()
    {
        return terrain.GetComponentsInChildren<Tile>();
    }

    public void LoadCity(API.City map)
    {
        try
        {
            var buildings = Resources.LoadAll<Building>("Building");
            var resources = Resources.LoadAll<Resource>("Resource");

            Destroy(terrain);
            terrain = new GameObject("Terrain");
            terrain.transform.SetParent(transform);

            tiles = new Dictionary<Vector2Int, Tile>();

            var rand = new System.Random(0);
            for (int i = 0; i < map.height; i++)
            {
                for (int j = 0; j < map.width; j++)
                {
                    var field = map.fields.First(f => f.x == j && f.y == i);
                    var tile = Instantiate<GameObject>(GetTilePrefab(field.fieldType));
                    tile.transform.SetParent(terrain.transform);
                    tile.transform.position = new Vector3(j * TILE_SIZE, 0, i * TILE_SIZE);
                    var comp = tile.AddComponent<Tile>();
                    comp.Field = field.fieldType;
                    comp.Resource = resources.FirstOrDefault(r => r.ApiType == field.resourceType);
                    comp.Building = buildings.FirstOrDefault(b => b.ApiType == field.buildingType);
                    comp.BuildingLevel = field.buildingLevel;
                    comp.X = j;
                    comp.Y = i;
                    tiles[new Vector2Int(j, i)] = comp;
                    var collider = tile.AddComponent<BoxCollider>();
                    collider.size = new Vector3(TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    collider.center = new Vector3(0, -TILE_SIZE / 2, 0);
                    var rot = rand.Next(4);
                    tile.transform.GetChild(0).Rotate(new Vector3(0, rot * 90, 0), Space.World);
                    tile.name = "Tile_" + j + "_" + i;

                    var building = comp.Building;
                    if (building)
                    {
                        var model = Instantiate(building.Prefab);
                        model.transform.SetParent(tile.transform, false);
                    }
                    else
                    {
                        var resource = comp.Resource;
                        if (resource)
                        {
                            var resourceModel = Instantiate<GameObject>(resource.Prefab);
                            resourceModel.transform.SetParent(tile.transform, false);
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }
    }

    private GameObject GetTilePrefab(FieldType type)
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

    public Tile GetTile(int x, int y)
    {
        return tiles[new Vector2Int(x, y)];
    }

}
