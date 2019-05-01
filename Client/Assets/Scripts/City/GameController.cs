using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CityLoader CityLoader;
    private IAPI api = new DummyAPI();
    public Player MyPlayer;
    private long? CurrentCityId;
    private DateTime? lastResourceUpdate = null;
    private int? tickDuration;

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
        Debug.Log("LoadCity\n" + JsonUtility.ToJson(res, true));
        CurrentCityId = res.City.Id;
        tickDuration = res.City.tickDuration;
        CityLoader.LoadCity(res.City);
        api.GetResources(res.City.Id, GetResources);
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
