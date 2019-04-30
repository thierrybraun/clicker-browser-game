using Model;
using System;

public interface IAPI
{
    void GetCity(long cityId, Action<City> callback);
    void CreateBuilding(long cityId, int building, int x, int y, Action<CreateBuildingResponse> callback);
}

public struct CreateBuildingResponse
{
    public bool Success;
    public string Error;
}