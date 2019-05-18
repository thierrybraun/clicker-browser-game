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
    public ResourceCollection Stash = new ResourceCollection();

    private GameController gameController;
    private DateTime NextResourceUpdate = DateTime.Now;
    private DateTime LastResourceUpdate = DateTime.Now;
    private GameState State = GameState.Instance;

    private void Start()
    {
        UI.UIController.Instance.Register(this);
        gameController = FindObjectOfType<GameController>();
        if (Building)
        {
            StartCoroutine("ResourceUpdateTimer");
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
        State.Api.GetStashForTile(State.CurrentCityId.Value, X, Y, OnGetStashForTile);
    }

    private void OnGetStashForTile(API.GetStashForTileResponse res)
    {
        Debug.Log("GetResources " + X + "," + Y + "\n" + JsonUtility.ToJson(res, true));
        NextResourceUpdate = DateTime.Now.AddSeconds(res.SecondsUntilNextUpdate);
        LastResourceUpdate = DateTime.Now;
        Stash = res.Resources.ToResourceCollection();

        StartCoroutine("ResourceUpdateTimer");
    }

    public TimeSpan GetTimeUntilNextUpdate()
    {
        return NextResourceUpdate.Subtract(DateTime.Now);
    }

    public float GetProgressUntilNextUpdate()
    {
        var total = (float)NextResourceUpdate.Subtract(LastResourceUpdate).TotalSeconds;
        var done = (float)NextResourceUpdate.Subtract(DateTime.Now).TotalSeconds;
        var progress = 1 - Mathf.Clamp01(done / total);
        return progress;
    }
}
