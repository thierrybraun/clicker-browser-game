using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public FieldType Field;
    public Resource Resource;
    public Building Building;

    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    /*
    private void OnGUI()
    {
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        Sprite sprite = null;
        switch (Building)
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
        if (Building.HasValue) Debug.Log(X + " " + Y + " " + Building + " " + sprite);
        if (sprite)
        {
            GUI.DrawTexture(new Rect(pos.x, pos.y, 64, 64), sprite.texture);
        }
    }
    */
    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Resource)
        {
            Building building = Resource.Building;

            if (building)
            {
                gameController.Build(building, X, Y);
            }
        }
    }
}
