using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text Food, Wood, Metal;

    private void Update()
    {
        if (GameState.Instance.MyPlayer != null)
        {
            Food.text = GameState.Instance.MyPlayer.Food + "";
            Wood.text = GameState.Instance.MyPlayer.Wood + "";
            Metal.text = GameState.Instance.MyPlayer.Metal + "";
        }
    }
}
