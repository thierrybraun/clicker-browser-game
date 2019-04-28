using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainProvider : MonoBehaviour
{
    public static int TILE_SIZE = 10;
    public GameObject Flat, Water, Hills;

    public GameObject GetTile(FieldType type)
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
}
