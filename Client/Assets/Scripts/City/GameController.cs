using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public CityLoader CityLoader;
    public WorldUI WorldUI;
    private IAPI api = new DummyAPI();
    public Player MyPlayer;
    private long? CurrentCityId;
    private DateTime? lastResourceUpdate = null;
    private int? tickDuration;
    public IList<Model.ResourceStash> ResourceCollection = new List<Model.ResourceStash>();

    private void Start()
    {
        if (!CityLoader)
        {
            Debug.LogError("GameController: CityLoader not found");
            return;
        }

        api.GetPlayer(0, LoadPlayer);
    }

    private void Update()
    {
        if (lastResourceUpdate.HasValue && CurrentCityId.HasValue && tickDuration.HasValue && DateTime.UtcNow.Subtract(lastResourceUpdate.Value) > TimeSpan.FromSeconds(tickDuration.Value))
        {
            api.GetResources(CurrentCityId.Value, GetResources);
        }
    }

    private void GetResources(GetResourcesResponse res)
    {
        Debug.Log("GetResources: " + res.LastResourceUpdate + "\n" + JsonUtility.ToJson(res, true));
        //lastResourceUpdate = DateTime.Parse(res.LastResourceUpdate);
        lastResourceUpdate = DateTime.UtcNow;
        ResourceCollection = res.Resources.ToList();
    }

    public void Collect(int x, int y)
    {
        api.CollectResources(CurrentCityId.Value, x, y, OnCollected);
    }

    private void OnCollected(CollectResourcesResponse res)
    {
        Debug.Log("Collect: \n" + JsonUtility.ToJson(res, true));
        var newStash = res.Resources;
        var oldStash = ResourceCollection.Where(s => s.X == newStash.X && s.Y == newStash.Y).First();
        ResourceCollection.Remove(oldStash);
        ResourceCollection.Add(newStash);
        MyPlayer = res.Player;
    }

    private void LoadPlayer(GetPlayerResponse resp)
    {
        Debug.Log("LoadPlayer\n" + JsonUtility.ToJson(resp, true));
        if (resp.Success)
        {
            MyPlayer = resp.Player;
            api.GetCityForPlayer(MyPlayer.Id, LoadCity);
        }
    }

    private void LoadCity(GetCityResponse res)
    {
        try
        {
            Debug.Log("LoadCity\n" + JsonUtility.ToJson(res, true));
            CurrentCityId = res.City.Id;
            tickDuration = res.City.tickDuration;
            CityLoader.LoadCity(res.City);
            WorldUI.Setup(res.City);
            api.GetResources(res.City.Id, GetResources);
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    public void Build(BuildingType building, int x, int y)
    {
        api.CreateBuilding(CurrentCityId.Value, (int)building, x, y, res =>
        {
            Debug.Log("Build\n" + JsonUtility.ToJson(res, true));
            if (res.Success)
            {
                api.GetCity(CurrentCityId.Value, LoadCity);
            }
        });
    }
}
