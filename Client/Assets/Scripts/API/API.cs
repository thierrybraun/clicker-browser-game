using System;
using UnityEngine;

namespace API
{
    public abstract class API : MonoBehaviour
    {
        private static API instance;

        public static API Instance { get { return instance; } }


        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        ///<value>user:pass, encoded in Base64</value>
        public abstract string Credentials { set; }
        public abstract void Authenticate(string user, string pass, Action<AuthenticateResponse> callback);
        public abstract void GetCity(long cityId, Action<GetCityResponse> callback);
        public abstract void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback);
        public abstract void GetPlayer(long playerId, Action<GetPlayerResponse> callback);
        public abstract void GetMe(Action<GetPlayerResponse> callback);
        public abstract void GetCityForPlayer(long playerId, Action<GetCityResponse> callback);
        public abstract void CollectResources(long currentCityId, int x, int y, Action<CollectResourcesResponse> callback);
        public abstract void GetStashForTile(long currentCityId, int x, int y, Action<GetStashForTileResponse> callback);
        public abstract void UpgradeBuilding(long currentCityId, int x, int y, int targetLevel, Action<UpgradeResponse> callback);
        public abstract void GetVersion(Action<GetVersionResponse> callback);
    }

    public struct AuthenticateResponse
    {
        public bool Success;
        public string Error;
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
        public Currency CityResources;
        public Currency Resources;
        public bool Success;
        public string Error;
    }

    public struct GetStashForTileResponse
    {
        public bool Success;
        public string Error;
        public Currency Resources;
        public int SecondsUntilNextUpdate;
        public int SecondsFromLastUpdate;
    }

    public struct UpgradeResponse
    {
        public bool Success;
        public string Error;
    }

    public struct GetVersionResponse
    {
        public string Version;
    }
}