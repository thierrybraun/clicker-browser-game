using Model;
using System;
using System.Collections.Generic;

public class DummyAPI : IAPI
{
    private const int width = 10, height = 10;
    private IDictionary<long, City> cities;
    private IDictionary<long, Player> players;

    public DummyAPI()
    {
        cities = new Dictionary<long, City>
        {
            { 0, CreateCity(0) }
        };

        players = new Dictionary<long, Player>
        {
            { 0, new Player { Id = 0, Name = "Player1" } }
        };
    }

    private City CreateCity(long id)
    {
        var city = new City();
        city.Id = id;
        Field[] fields = new Field[10 * 10];
        var random = new Random((int)id);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
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
        city.width = width;
        city.height = height;
        city.fields = fields;
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
}
