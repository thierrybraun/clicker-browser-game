using System.Collections.Generic;
using API;
using System;

public class GameState
{
    private static GameState instance;

    public long? CurrentCityId;
    public Player MyPlayer;
    public DateTime? LastResourceUpdate = null;
    public int? TickDuration;

    private GameState()
    {

    }

    public static GameState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameState();
            }
            return instance;
        }
    }

    public void LoadCity(City city)
    {
        CurrentCityId = city.Id;
        TickDuration = city.tickDuration;
    }
}
