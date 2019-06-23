using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Tile : MonoBehaviour
{
    public int X, Y;
    public FieldType Field;
    public Resource Resource;
    public int BuildingLevel;
    public Currency Stash = new Currency();

    private Building building;
    private GameController gameController;
    private DateTime NextResourceUpdate = DateTime.Now;
    private DateTime LastResourceUpdate = DateTime.Now;
    private DateTime LastStashChange = DateTime.Now;
    private GameState State = GameState.Instance;
    private Coroutine resourceUpdateTimer;

    public Building Building
    {
        get => building;
        set
        {
            building = value;
            UI.UIController.Instance.Register(this);
            if (resourceUpdateTimer == null) resourceUpdateTimer = StartCoroutine("ResourceUpdateTimer");
        }
    }

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (Building)
        {
            UI.UIController.Instance.Register(this);
            resourceUpdateTimer = StartCoroutine("ResourceUpdateTimer");
        }
    }

    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        UI.UIController.Instance.HideMenues();
        if (Building)
        {
            UI.UIController.Instance.ShowBuilding(this);
        }
        else if (Resource)
        {
            Building building = Resource.Building;

            if (building)
            {
                UI.UIController.Instance.ShowBuildMenu(this);
            }
        }
    }

    private IEnumerator ResourceUpdateTimer()
    {
        var delay = Math.Max(0, (float)Math.Ceiling((NextResourceUpdate.Subtract(DateTime.Now)).TotalSeconds));
        yield return new WaitForSeconds(delay);
        API.API.Instance.GetStashForTile(State.CurrentCityId.Value, X, Y, OnGetStashForTile);
    }

    private void OnGetStashForTile(API.GetStashForTileResponse res)
    {
        // Debug.Log("GetResources " + X + "," + Y + "\n" + JsonUtility.ToJson(res, true));
        NextResourceUpdate = DateTime.Now.AddSeconds(res.SecondsUntilNextUpdate);
        LastResourceUpdate = DateTime.Now;
        LastStashChange = DateTime.Now.Subtract(TimeSpan.FromSeconds(res.SecondsFromLastUpdate));
        Stash = res.Resources;

        resourceUpdateTimer = StartCoroutine("ResourceUpdateTimer");
    }

    public TimeSpan GetTimeUntilNextUpdate()
    {
        return NextResourceUpdate.Subtract(DateTime.Now);
    }

    public float GetProgressUntilNextUpdate()
    {
        var total = (float)NextResourceUpdate.Subtract(LastStashChange).TotalSeconds;
        var done = (float)NextResourceUpdate.Subtract(DateTime.Now).TotalSeconds;
        var progress = 1 - Mathf.Clamp01(done / total);
        return progress;
    }
}
