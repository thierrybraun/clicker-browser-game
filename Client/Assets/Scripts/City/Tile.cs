using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public FieldType Field;
    public Resource Resource;
    public Building Building;
    public Model.ResourceStash Stash = new Model.ResourceStash();

    private GameController gameController;
    private DateTime NextResourceUpdate = DateTime.UtcNow;
    private GameState State = GameState.Instance;

    private void Start()
    {
        WorldUI.Instance.Register(this);
        gameController = FindObjectOfType<GameController>();
        if (Building)
        {
            StartCoroutine("ResourceUpdateTimer");
        }
    }

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

    private IEnumerator ResourceUpdateTimer()
    {
        var delay = Math.Max(0, (float)Math.Ceiling((NextResourceUpdate.Subtract(DateTime.Now)).TotalSeconds));
        yield return new WaitForSeconds(delay);
        State.Api.GetStashForTile(State.CurrentCityId.Value, X, Y, OnGetStashForTile);
    }

    private void OnGetStashForTile(API.GetStashForTileResponse res)
    {
        Debug.Log("GetResources " + X + "," + Y + "\n" + JsonUtility.ToJson(res, true));
        NextResourceUpdate = DateTime.Now.AddSeconds(res.SecondsUntilNextUpdate);
        Stash = res.Resources;

        StartCoroutine("ResourceUpdateTimer");
    }
}
