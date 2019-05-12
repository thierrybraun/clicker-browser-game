using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public FieldType Field;
    public Resource Resource;
    public Building Building;

    private GameController gameController;
    private DateTime NextResourceUpdate = DateTime.UtcNow;
    private GameState State = GameState.Instance;

    private void Start()
    {
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

            if (Building)
            {
                Debug.Log(X + "," + Y + ":" + JsonUtility.ToJson(State.ResourceCollection.FirstOrDefault(s => s.X == X && s.Y == Y)));
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
        var stash = State.ResourceCollection.FirstOrDefault(s => s.X == X && s.Y == Y);
        State.ResourceCollection.Remove(stash);
        res.Resources.X = stash.X;
        res.Resources.Y = stash.Y;
        State.ResourceCollection.Add(res.Resources);

        StartCoroutine("ResourceUpdateTimer");
    }
}
