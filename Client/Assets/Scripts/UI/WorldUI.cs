using UnityEngine;
using System.Collections.Generic;

public class WorldUI : MonoBehaviour
{
    public static WorldUI Instance;
    public GameObject Collectible;
    public Sprite Food, Wood, Metal;

    private IDictionary<Tile, Collectible> Collectibles = new Dictionary<Tile, Collectible>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void Reset()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void Register(Tile tile)
    {
        Sprite sprite = tile.Building?.CollectionSprite;

        if (sprite)
        {
            Collectible collectible = Instantiate(Collectible, transform).GetComponent<Collectible>();
            collectible.name = "CollectionPopup_" + tile.X + "_" + tile.Y;            
            collectible.Sprite = sprite;
            collectible.Tile = tile;

            if (Collectibles.ContainsKey(tile))
            {
                DestroyImmediate(Collectibles[tile].gameObject);
                Collectibles.Remove(tile);
            }
            Collectibles.Add(tile, collectible);
        }
    }    
}
