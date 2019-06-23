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
        tiles = new Dictionary<Vector2Int, Tile>();
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

            var rand = new System.Random(0);
            for (int i = 0; i < map.height; i++)
            {
                for (int j = 0; j < map.width; j++)
                {
                    var field = map.fields.First(f => f.x == j && f.y == i);
                    var tile = GetTile(j, i);
                    if (tile == null)
                    {
                        tile = InstantiateTile(j, i, field.fieldType);
                        tiles[new Vector2Int(j, i)] = tile;
                        tile.Field = field.fieldType;
                        tile.X = j;
                        tile.Y = i;
                        tile.name = "Tile_" + j + "_" + i;
                        tile.Resource = resources.FirstOrDefault(r => r.ApiType == field.resourceType);
                        if (tile.Resource)
                        {
                            var resourceModel = Instantiate<GameObject>(tile.Resource.Prefab, tile.transform, false);
                            resourceModel.name = "Resource";
                        }

                    }
                    tile.BuildingLevel = field.buildingLevel;

                    var rot = rand.Next(4);
                    tile.transform.GetChild(0).rotation = Quaternion.Euler(-90, rot * 90, 0);

                    if (field.buildingType != API.BuildingType.None)
                    {
                        if (!tile.Building || tile.Building.ApiType != field.buildingType)
                        {
                            tile.Building = buildings.FirstOrDefault(b => b.ApiType == field.buildingType);
                            var model = Instantiate(tile.Building.Prefab, tile.transform, false);

                            var resource = tile.transform.Find("Resource");
                            if (resource)
                            {
                                Destroy(resource.gameObject);
                            }
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

    private Tile InstantiateTile(int x, int y, FieldType type)
    {
        var tileGo = Instantiate<GameObject>(GetTilePrefab(type));
        tileGo.transform.SetParent(terrain.transform);
        tileGo.transform.position = new Vector3(x * TILE_SIZE, 0, y * TILE_SIZE);
        var collider = tileGo.AddComponent<BoxCollider>();
        collider.size = new Vector3(TILE_SIZE, TILE_SIZE, TILE_SIZE);
        collider.center = new Vector3(0, -TILE_SIZE / 2, 0);
        return tileGo.AddComponent<Tile>();
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
        Tile tile = null;
        tiles.TryGetValue(new Vector2Int(x, y), out tile);
        return tile;
    }

}
