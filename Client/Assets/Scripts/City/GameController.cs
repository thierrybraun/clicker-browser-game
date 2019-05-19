﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using API;

public class GameController : MonoBehaviour
{
    public CityLoader CityLoader;
    private GameState State = GameState.Instance;

    private void Start()
    {
        if (!CityLoader)
        {
            Debug.LogError("GameController: CityLoader not found");
            return;
        }

        State.Api.GetPlayer(0, LoadPlayer);
    }

    public Tile GetTile(int x, int y)
    {
        return CityLoader.GetTile(x, y);
    }

    private void LoadPlayer(GetPlayerResponse resp)
    {
        Debug.Log("LoadPlayer\n" + JsonUtility.ToJson(resp, true));
        if (resp.Success)
        {
            State.MyPlayer = resp.Player;
            State.Api.GetCityForPlayer(State.MyPlayer.Id, LoadCity);
        }
    }

    private void LoadCity(GetCityResponse res)
    {
        try
        {
            Debug.Log("LoadCity\n" + JsonUtility.ToJson(res, true));
            State.LoadCity(res.City);
            CityLoader.LoadCity(res.City);
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    public void Build(Building building, int x, int y)
    {
        State.Api.CreateBuilding(State.CurrentCityId.Value, (int)building.ApiType, x, y, res =>
        {
            Debug.Log("Build\n" + JsonUtility.ToJson(res, true));
            if (res.Success)
            {
                State.MyPlayer = res.Player;
                State.Api.GetCity(State.CurrentCityId.Value, LoadCity);
            }
        });
    }

    public void UpgradeBuilding(Tile tile)
    {
        GameState.Instance.Api.UpgradeBuilding(GameState.Instance.CurrentCityId.Value, tile.X, tile.Y, UpgradeHandler);
    }

    public void UpgradeHandler(API.UpgradeResponse response)
    {
        Debug.Log("UpgradeBuilding: " + JsonUtility.ToJson(response, true));
        if (response.Success)
        {
            State.Api.GetCity(State.CurrentCityId.Value, LoadCity);
        }
        else
        {
            Debug.LogError(response.Error);
        }
    }
}
