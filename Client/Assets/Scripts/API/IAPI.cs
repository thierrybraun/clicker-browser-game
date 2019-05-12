using Model;
using System;

namespace API
{
    public interface IAPI
    {
        void GetCity(long cityId, Action<GetCityResponse> callback);
        void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback);
        void GetPlayer(long playerId, Action<GetPlayerResponse> callback);
        void GetCityForPlayer(long playerId, Action<GetCityResponse> callback);        
        void CollectResources(long currentCityId, int x, int y, Action<CollectResourcesResponse> callback);
        void GetStashForTile(long currentCityId, int x, int y, Action<GetStashForTileResponse> callback);
    }

    public struct GetCityResponse
    {
        public bool Success;
        public string Error;
        public City City;
    }

    public struct CreateBuildingResponse
    {
        public bool Success;
        public string Error;
    }

    public struct GetPlayerResponse
    {
        public bool Success;
        public string Error;
        public Player Player;
    }    

    public struct CollectResourcesResponse
    {
        public ResourceStash Resources;
        public Player Player;
        public bool Success;
        public string Error;
    }

    public struct GetStashForTileResponse
    {
        public bool Success;
        public string Error;
        public ResourceStash Resources;
        public int SecondsUntilNextUpdate;
    }
}