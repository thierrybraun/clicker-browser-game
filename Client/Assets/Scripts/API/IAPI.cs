using Model;
using System;

public interface IAPI
{
    void GetCity(long id, Action<City> callback);
}
