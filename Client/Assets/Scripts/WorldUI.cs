using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    public GameObject Collectible;
    public Sprite Food, Wood, Metal;

    public void Setup(Model.City city)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        foreach (var f in city.fields)
        {            
            Sprite sprite = null;
            switch (f.buildingType)
            {
                case BuildingType.Applefarm:
                    sprite = Food;
                    break;
                case BuildingType.House:
                    break;
                case BuildingType.Fishingboat:
                    sprite = Food;
                    break;
                case BuildingType.Lumberjack:
                    sprite = Wood;
                    break;
                case BuildingType.Mine:
                    sprite = Metal;
                    break;
            }

            if (sprite)
            {
                GameObject collectionPopup = Instantiate(Collectible, transform);
                collectionPopup.name = "CollectionPopup_" + f.x + "_" + f.y;
                var collectible = collectionPopup.GetComponent<Collectible>();
                collectible.Sprite = sprite;
                collectible.Location = new Vector2Int(f.x, f.y);
                collectionPopup.transform.position = new Vector3(f.x * 10, 0, f.y * 10);
                collectionPopup.transform.localScale = Vector3.one / 10f;
            }
        }
    }
}
