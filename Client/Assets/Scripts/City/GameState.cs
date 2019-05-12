using System.Collections.Generic;
using API;
using System;
using Model;

public class GameState
{
    private static GameState instance;

    public IList<Model.ResourceStash> ResourceCollection = new List<Model.ResourceStash>();
    public long? CurrentCityId;
    public Player MyPlayer;
    public IAPI Api = new DummyAPI();
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

        ResourceCollection = new List<Model.ResourceStash>();
        foreach (var f in city.fields)
        {
            ResourceCollection.Add(new ResourceStash
            {
                X = f.x,
                Y = f.y
            });
        }
    }
}
