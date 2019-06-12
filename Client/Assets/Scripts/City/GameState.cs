using System.Collections.Generic;
using API;
using System;

public class GameState
{
    private static GameState instance;

    public Player MyPlayer;
    public City? CurrentCity;
    public DateTime? LastResourceUpdate = null;

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

    public long? CurrentCityId { get => CurrentCity?.Id; }
    public int? TickDuration { get => CurrentCity?.tickDuration; }

    public void LoadCity(City city)
    {
        CurrentCity = city;
    }
}
