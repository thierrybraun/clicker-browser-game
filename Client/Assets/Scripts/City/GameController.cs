using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CityLoader CityLoader;
    private IAPI api = new DummyAPI();
    public Player MyPlayer;
    private long CurrentCityId;

    private void Start()
    {
        if (!CityLoader)
        {
            Debug.LogError("GameController: CityLoader not found");
            return;
        }

        api.GetPlayer(0, LoadPlayer);
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
        CityLoader.LoadCity(res.City);
    }

    public void Build(BuildingType building, int x, int y)
    {
        api.CreateBuilding(CurrentCityId, (int)building, x, y, res =>
        {
            Debug.Log("Build\n" + JsonUtility.ToJson(res, true));
            if (res.Success)
            {
                api.GetCity(CurrentCityId, LoadCity);
            }
        });
    }
}
