using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CityLoader CityLoader;
    private IAPI api = new DummyAPI();
    private long CurrentCityId = 0;

    private void Start()
    {
        if (!CityLoader)
        {
            Debug.LogError("GameController: CityLoader not found");
            return;
        }

        api.GetCity(CurrentCityId, CityLoader.LoadCity);
    }

    public void Build(BuildingType building, int x, int y)
    {
        api.CreateBuilding(CurrentCityId, (int)building, x, y, res =>
        {
            Debug.Log("Success: " + res.Success);
            if (!res.Success)
            {
                Debug.Log(res.Error);
            }
            api.GetCity(CurrentCityId, CityLoader.LoadCity);
        });
    }
}
