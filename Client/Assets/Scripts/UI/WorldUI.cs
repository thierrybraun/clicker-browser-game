using UnityEngine;
using Model;

public class WorldUI : MonoBehaviour
{
    public GameObject Collectible;
    public Sprite Food, Wood, Metal;

    public void Setup(Tile[] tiles)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        foreach (var tile in tiles)
        {
            Sprite sprite = tile.Building?.CollectionSprite;

            if (sprite)
            {
                GameObject collectionPopup = Instantiate(Collectible, transform);
                collectionPopup.name = "CollectionPopup_" + tile.X + "_" + tile.Y;
                var collectible = collectionPopup.GetComponent<Collectible>();
                collectible.Sprite = sprite;
                collectible.Location = new Vector2Int(tile.X, tile.Y);
                collectionPopup.transform.position = new Vector3(tile.X * 10, 0, tile.Y * 10);
                collectionPopup.transform.localScale = Vector3.one / 10f;
            }
        }
    }
}
