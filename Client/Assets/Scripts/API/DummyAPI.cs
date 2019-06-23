using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace API
{
    public class DummyAPI : API
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

        public override string Credentials { set { } }

        public DummyAPI()
        {
            cities = new Dictionary<long, City>
        {
            { 0, CreateCity(0) }
        };

            players = new Dictionary<long, Player>
        {
            { 0, new Player { id = 0, name = "Player1" } }
        };
        }

        private City CreateCity(long id)
        {
            buildings.Add(id, new DbBuilding[height, width]);
            Field[] fields = new Field[10 * 10];
            var random = new System.Random((int)id);
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
                id = id,
                width = width,
                height = height,
                fields = fields,
                tickDuration = tickDuration,
                wood = 100,
                metal = 100
            };
            return city;
        }

        public override void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback)
        {
            try
            {
                var city = cities[cityId];
                var buildingType = ((BuildingType[])Enum.GetValues(typeof(BuildingType)))[building];
                var dbBuilding = buildings[cityId][y, x];

                var cost = UnityEngine.Resources.Load<Building>("Building/" + buildingType.ToString()).BuildCostFunction.GetCost(1);

                var player = players[cityId];
                if (city.wood < cost.Wood || city.metal < cost.Metal || city.food < cost.Food)
                {
                    callback(new CreateBuildingResponse
                    {
                        Error = "Not enough resources",
                        Success = false
                    });
                }
                else
                {
                    city.food -= cost.Food;
                    city.wood -= cost.Wood;
                    city.metal -= cost.Metal;
                    players[cityId] = player;

                    city.fields[y * height + x].buildingType = buildingType;
                    city.fields[y * height + x].buildingLevel = 1;
                    cities[cityId] = city;
                    buildings[cityId][y, x] = new DbBuilding
                    {
                        Stash = new ResourceStash(),
                        BuildDate = DateTime.UtcNow,
                        LastQuery = DateTime.UtcNow
                    };

                    callback(new CreateBuildingResponse
                    {
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

        public override void GetCity(long cityId, Action<GetCityResponse> callback)
        {
            callback(new GetCityResponse { Success = true, City = cities[cityId] });
        }

        public override void GetPlayer(long playerId, Action<GetPlayerResponse> callback)
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

        public override void GetCityForPlayer(long playerId, Action<GetCityResponse> callback)
        {
            callback(new GetCityResponse { Success = true, City = cities[playerId] });
        }

        private int GetPassedTicks(DateTime last, DateTime now)
        {
            return now.Subtract(last).Seconds / tickDuration;
        }

        public override void CollectResources(long currentCityId, int x, int y, Action<CollectResourcesResponse> callback)
        {
            var city = cities[currentCityId];
            var field = cities[currentCityId].fields.Where(f => f.x == x && f.y == y);

            var building = buildings[currentCityId][y, x];

            var player = players[currentCityId];
            city.food += building.Stash.Food;
            city.wood += building.Stash.Wood;
            city.metal += building.Stash.Metal;

            building.Stash.Food = 0;
            building.Stash.Wood = 0;
            building.Stash.Metal = 0;
            building.Stash.X = x;
            building.Stash.Y = y;

            buildings[currentCityId][y, x] = building;
            cities[currentCityId] = city;

            callback(new CollectResourcesResponse
            {
                Success = true,
                Resources = building.Stash,
                CityResources = city.Currency
            });
        }

        public override void GetStashForTile(long cityId, int x, int y, Action<GetStashForTileResponse> callback)
        {
            var building = buildings[cityId][y, x];
            var dateNow = DateTime.UtcNow;
            var ticks = GetPassedTicks(building.LastQuery, dateNow);
            building.LastQuery = building.LastQuery.AddSeconds(tickDuration * ticks);
            int secondsUntilNextUpdate = (int)Math.Ceiling(building.LastQuery.Add(TimeSpan.FromSeconds(tickDuration)).Subtract(dateNow).TotalSeconds);
            int secondsFromLastQuery = dateNow.Subtract(building.LastQuery).Seconds;

            var city = cities[cityId];
            var tile = city.fields.First(f => f.x == x && f.y == y);

            var production = UnityEngine.Resources.Load<Building>("Building/" + tile.buildingType.ToString()).ProductionFunction.GetProduction(tile.buildingLevel);

            switch (tile.buildingType)
            {
                case BuildingType.Applefarm:
                    building.Stash.Food += ticks * production.Food;
                    break;
                case BuildingType.Fishingboat:
                    building.Stash.Food += ticks * production.Food;
                    break;
                case BuildingType.House:
                    //Food += GetPassedTicks(lastQuery, lastUpdateTime);
                    break;
                case BuildingType.Lumberjack:
                    building.Stash.Wood += ticks * production.Wood;
                    break;
                case BuildingType.Mine:
                    building.Stash.Metal += ticks * production.Metal;
                    break;
            }

            buildings[cityId][y, x] = building;

            callback(new GetStashForTileResponse
            {
                Success = true,
                SecondsUntilNextUpdate = secondsUntilNextUpdate,
                SecondsFromLastUpdate = secondsFromLastQuery,
                Resources = building.Stash
            });
        }

        public override void UpgradeBuilding(long currentCityId, int x, int y, int targetLevel, Action<UpgradeResponse> callback)
        {
            var city = cities[currentCityId];
            var building = buildings[currentCityId][y, x];
            var player = players[currentCityId];
            var buildingType = cities[currentCityId].fields.First(f => f.x == x && f.y == y).buildingType;
            var field = cities[currentCityId].fields[y * height + x];

            var cost = UnityEngine.Resources.Load<Building>("Building/" + buildingType.ToString()).BuildCostFunction.GetCostRange(field.buildingLevel, targetLevel);

            if (city.Currency < cost)
            {
                callback(new UpgradeResponse
                {
                    Error = "Not enough resources",
                    Success = false
                });
            }
            else
            {
                city.Currency -= cost;
                players[currentCityId] = player;

                buildings[currentCityId][y, x] = building;
                city.fields[y * height + x].buildingLevel = targetLevel;
                cities[currentCityId] = city;

                callback(new UpgradeResponse
                {
                    Success = true
                });
            }
        }

        public override void Authenticate(string user, string pass, Action<AuthenticateResponse> callback)
        {
            callback(new AuthenticateResponse
            {
                Success = user == "Player1",
                Error = user == "Player1" ? null : "Use 'Player1' as username"
            });
        }

        public override void GetMe(Action<GetPlayerResponse> callback)
        {
            GetPlayer(0, callback);
        }

        public override void GetVersion(Action<GetVersionResponse> callback)
        {
            callback(new GetVersionResponse
            {
                Version = "local"
            });
        }
    }
}