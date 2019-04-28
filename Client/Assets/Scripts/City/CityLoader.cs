using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using System.Linq;

public class CityLoader : MonoBehaviour
{
    public TerrainProvider TerrainProvider;

    private void Start()
    {
        if (!TerrainProvider)
        {
            Debug.LogError("TerrainProvider not found");
            return;
        }
    }

    public void LoadCity(City map)
    {
        var rand = new System.Random(0);
        for (int i = 0; i < map.height; i++)
        {
            for (int j = 0; j < map.width; j++)
            {
                var field = map.fields.First(f => f.x == j && f.y == i);
                var tile = Instantiate<GameObject>(TerrainProvider.GetTile(field.fieldType));
                tile.transform.position = new Vector3(j * TerrainProvider.TILE_SIZE, 0, i * TerrainProvider.TILE_SIZE);
                var rot = rand.Next(4);
                tile.transform.Rotate(new Vector3(0, 0, rot * 90));
                tile.name = "Tile_" + j + "_" + i;
            }
        }
    }
}
