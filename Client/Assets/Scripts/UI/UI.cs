using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public BuildingDetail BuildingDetail;
    public Text Food, Wood, Metal;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Update()
    {
        var State = GameState.Instance;
        if (State.MyPlayer != null)
        {
            Food.text = State.MyPlayer.Food + "";
            Wood.text = State.MyPlayer.Wood + "";
            Metal.text = State.MyPlayer.Metal + "";
        }
    }

    public void ShowBuilding(Tile tile)
    {
        if (tile.Building)
        {
            BuildingDetail.Tile = tile;
            BuildingDetail.gameObject.SetActive(true);
        }
    }
}
