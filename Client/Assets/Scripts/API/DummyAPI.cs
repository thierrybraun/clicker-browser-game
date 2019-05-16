using System;
using System.Collections.Generic;
using System.Linq;

namespace API
{
    public class DummyAPI : IAPI
    {
        private struct DbBuilding
        {
            public ResourceStash Stash;
            public DateTime LastQuery;
            public DateTime BuildDate;
        }

        private const int width = 10, height = 10;
        private IDictionary<long, City> cities;
        private IDictionary<long, Player> players;
        private IDictionary<long, DbBuilding[,]> buildings = new Dictionary<long, DbBuilding[,]>();
        private const int tickDuration = 10;

        public DummyAPI()
        {
            cities = new Dictionary<long, City>
        {
            { 0, CreateCity(0) }
        };

            players = new Dictionary<long, Player>
        {
            { 0, new Player { Id = 0, Name = "Player1", Wood = 10 } }
        };
        }

        private City CreateCity(long id)
        {
            buildings.Add(id, new DbBuilding[height, width]);
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
                var buildingType = ((BuildingType[])Enum.GetValues(typeof(BuildingType)))[building];
                var cost = UnityEngine.Resources.Load<Building>("Building/" + buildingType.ToString()).BuildCostFunction.GetCost(1);

                var player = players[cityId];
                if (player.Wood < cost.Wood || player.Metal < cost.Metal || player.Food < cost.Food)
                {
                    callback(new CreateBuildingResponse
                    {
                        Error = "Not enough resources",
                        Success = false
                    });
                }
                else
                {
                    player.Food -= cost.Food;
                    player.Wood -= cost.Wood;
                    player.Metal -= cost.Metal;
                    players[cityId] = player;

                    city.fields[y * height + x].buildingType = buildingType;
                    buildings[cityId][y, x] = new DbBuilding
                    {
                        Stash = new ResourceStash(),
                        BuildDate = DateTime.UtcNow,
                        LastQuery = DateTime.UtcNow
                    };

                    callback(new CreateBuildingResponse
                    {
                        Player = player,
                        Success = true
                    });
                }
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

        private int GetPassedTicks(DateTime last, DateTime now)
        {
            return now.Subtract(last).Seconds / tickDuration;
        }

        public void CollectResources(long currentCityId, int x, int y, Action<CollectResourcesResponse> callback)
        {
            var field = cities[currentCityId].fields.Where(f => f.x == x && f.y == y);

            var building = buildings[currentCityId][y, x];

            var player = players[currentCityId];
            player.Food += building.Stash.Food;
            player.Wood += building.Stash.Wood;
            player.Metal += building.Stash.Metal;

            building.Stash.Food = 0;
            building.Stash.Wood = 0;
            building.Stash.Metal = 0;
            building.Stash.X = x;
            building.Stash.Y = y;

            buildings[currentCityId][y, x] = building;

            callback(new CollectResourcesResponse
            {
                Success = true,
                Resources = building.Stash,
                Player = player
            });
        }

        public void GetStashForTile(long cityId, int x, int y, Action<GetStashForTileResponse> callback)
        {
            var building = buildings[cityId][y, x];
            var dateNow = DateTime.UtcNow;
            var ticks = GetPassedTicks(building.LastQuery, dateNow);
            building.LastQuery = building.LastQuery.AddSeconds(tickDuration * ticks);
            int secondsUntilNextUpdate = (int)Math.Ceiling(building.LastQuery.Add(TimeSpan.FromSeconds(tickDuration)).Subtract(dateNow).TotalSeconds);

            var city = cities[cityId];
            var tile = city.fields.First(f => f.x == x && f.y == y);

            switch (tile.buildingType)
            {
                case BuildingType.Applefarm:
                    building.Stash.Food += ticks;
                    break;
                case BuildingType.Fishingboat:
                    building.Stash.Food += ticks;
                    break;
                case BuildingType.House:
                    //Food += GetPassedTicks(lastQuery, lastUpdateTime);
                    break;
                case BuildingType.Lumberjack:
                    building.Stash.Wood += ticks;
                    break;
                case BuildingType.Mine:
                    building.Stash.Metal += ticks;
                    break;
            }

            buildings[cityId][y, x] = building;

            callback(new GetStashForTileResponse
            {
                Success = true,
                SecondsUntilNextUpdate = secondsUntilNextUpdate,
                Resources = building.Stash
            });
        }
    }
}