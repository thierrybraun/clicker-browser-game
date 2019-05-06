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

        if (State.LastResourceUpdate.HasValue && State.CurrentCityId.HasValue && State.TickDuration.HasValue)
        {
            Time.text = "Time until next tick:\n";
            Time.text += (TimeSpan.FromSeconds(State.TickDuration.Value) - DateTime.UtcNow.Subtract(State.LastResourceUpdate.Value)).ToString(("mm':'ss"));
        }
        else
        {
            Time.text = "";
        }
    }
}
