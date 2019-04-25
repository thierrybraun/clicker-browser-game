using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void LoadTerrain()
    {
        int mapSize = 10;
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                var tile = Instantiate<GameObject>(TerrainProvider.Flat);
                tile.transform.position = new Vector3(j * TerrainProvider.TILE_SIZE, 0, i * TerrainProvider.TILE_SIZE);
                tile.name = "Tile_" + j + "_" + i;
            }
        }
    }
}
