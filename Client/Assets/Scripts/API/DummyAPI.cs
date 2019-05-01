using Model;
using System;
using System.Collections.Generic;

public class DummyAPI : IAPI
{
    private const int width = 10, height = 10;
    private IDictionary<long, City> cities;
    private IDictionary<long, DateTime> lastResourceQuery = new Dictionary<long, DateTime>();
    private IDictionary<long, Player> players;
    private IDictionary<long, ResourceStash[,]> resourceStashes = new Dictionary<long, ResourceStash[,]>();
    private const int tickDuration = 10;

    public DummyAPI()
    {
        cities = new Dictionary<long, City>
        {
            { 0, CreateCity(0) }
        };

        players = new Dictionary<long, Player>
        {
            { 0, new Player { Id = 0, Name = "Player1", Wood = 100 } }
        };
    }

    private City CreateCity(long id)
    {
        resourceStashes.Add(id, new ResourceStash[height, width]);
        Field[] fields = new Field[10 * 10];
        var random = new Random((int)id);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                resourceStashes[id][i, j] = new ResourceStash();
                var types = (FieldType[])Enum.GetValues(typeof(FieldType));

                var field = new Field();
                field.x = j;
                field.y = i;
                field.fieldType = types[random.Next() % types.Length];

                switch (field.fieldType)
                {
                    case FieldType.Plain:
                        if (random.NextDouble() < 0.3d)
                        {
                            field.resourceType = ResourceType.Apples;
                        }
                        else if (random.NextDouble() > 0.6d)
                        {
                            field.resourceType = ResourceType.Forest;
                        }
                        break;
                    case FieldType.Water:
                        if (random.NextDouble() > 0.5d)
                        {
                            field.resourceType = ResourceType.Fish;
                        }
                        break;
                    case FieldType.Hills:
                        if (random.NextDouble() > 0.5d)
                        {
                            field.resourceType = ResourceType.Ore;
                        }
                        break;
                }

                fields[i * height + j] = field;
            }
        }

        var city = new City
        {
            Id = id,
            width = width,
            height = height,
            fields = fields,
            tickDuration = tickDuration
        };
        return city;
    }

    public void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback)
    {
        try
        {
            var city = cities[cityId];
            city.fields[y * height + x].buildingType = ((BuildingType[])Enum.GetValues(typeof(BuildingType)))[building];

            callback(new CreateBuildingResponse
            {
                Success = true
            });
        }
        catch (Exception e)
        {
            callback(new CreateBuildingResponse
            {
                Success = false,
                Error = e.Message
            });
        }
    }

    public void GetCity(long cityId, Action<GetCityResponse> callback)
    {
        callback(new GetCityResponse { Success = true, City = cities[cityId] });
    }

    public void GetPlayer(long playerId, Action<GetPlayerResponse> callback)
    {
        try
        {
            var p = players[playerId];

            callback(new GetPlayerResponse
            {
                Success = true,
                Player = p
            });
        }
        catch (Exception e)
        {
            callback(new GetPlayerResponse
            {
                Success = false,
                Error = e.Message
            });
        }
    }

    public void GetCityForPlayer(long playerId, Action<GetCityResponse> callback)
    {
        callback(new GetCityResponse { Success = true, City = cities[playerId] });
    }

    public void GetResources(long cityId, Action<GetResourcesResponse> callback)
    {
        var dateNow = DateTime.UtcNow;
        var lastUpdateTime = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute, dateNow.Second / tickDuration * tickDuration);

        DateTime lastQuery;
        if (!lastResourceQuery.TryGetValue(cityId, out lastQuery))
        {
            lastQuery = lastUpdateTime;
            lastResourceQuery.Add(cityId, lastUpdateTime);
        }
        else
        {
            lastResourceQuery[cityId] = lastUpdateTime;
        }

        var city = cities[cityId];

        var res = new List<ResourceStash>();
        foreach (var tile in city.fields)
        {
            if (tile.buildingType.HasValue)
            {
                var stash = resourceStashes[cityId][tile.y, tile.x];

                switch (tile.buildingType.Value)
                {
                    case BuildingType.Applefarm:
                        stash.Food += GetPassedTicks(lastUpdateTime, lastQuery);
                        break;
                    case BuildingType.Fishingboat:
                        stash.Food += GetPassedTicks(lastUpdateTime, lastQuery);
                        break;
                    case BuildingType.House:
                        //Food += GetPassedTicks(lastQuery, lastUpdateTime);
                        break;
                    case BuildingType.Lumberjack:
                        stash.Wood += GetPassedTicks(lastUpdateTime, lastQuery);
                        break;
                    case BuildingType.Mine:
                        stash.Metal += GetPassedTicks(lastUpdateTime, lastQuery);
                        break;
                }

                resourceStashes[cityId][tile.y, tile.x] = stash;

                var d = new ResourceStash
                {
                    X = tile.x,
                    Y = tile.y,
                    Wood = stash.Wood,
                    Food = stash.Food,
                    Metal = stash.Metal
                };
                res.Add(d);
            }
        }

        callback(new GetResourcesResponse
        {
            Success = true,
            LastResourceUpdate = lastUpdateTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
            Resources = res.ToArray()
        });
    }

    private int GetPassedTicks(DateTime last, DateTime now)
    {
        return last.Subtract(now).Seconds / tickDuration;
    }
    
}
