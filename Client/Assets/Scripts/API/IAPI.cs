using Model;
using System;

public interface IAPI
{
    void GetCity(long cityId, Action<GetCityResponse> callback);
    void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback);
    void GetPlayer(long playerId, Action<GetPlayerResponse> callback);
    void GetCityForPlayer(long playerId, Action<GetCityResponse> callback);
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