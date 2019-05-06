using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameController GameController;

    public Text Food, Wood, Metal;

    private void Update()
    {
        if (GameController.MyPlayer != null)
        {
            Food.text = GameController.MyPlayer.Food + "";
            Wood.text = GameController.MyPlayer.Wood + "";
            Metal.text = GameController.MyPlayer.Metal + "";
        }
    }
}
