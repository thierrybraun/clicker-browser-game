using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text Food, Wood, Metal, Time;

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
}
