using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CityLoader CityLoader;
    private IAPI api = new DummyAPI();

    private void Start()
    {
        if (!CityLoader)
        {
            Debug.LogError("GameController: CityLoader not found");
            return;
        }

        api.GetCity(0, CityLoader.LoadCity);
    }
}
