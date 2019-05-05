using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Collectible : MonoBehaviour
{
    public Vector2Int Location;

    private GameController gameController;
    public Text text;
    public Image icon;
    public GameObject background;

    public Sprite Sprite
    {
        set
        {
            icon.sprite = value;
        }
    }

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        transform.localRotation = Quaternion.Euler(new Vector3(45f, 45f, 0f));
    }

    private void Update()
    {        
        var stash = gameController.ResourceCollection.Where(s => s.X == Location.x && s.Y == Location.y).FirstOrDefault();
        background.SetActive(true);
        
        if (stash.Food > 0)
        {
            text.text = "+" + stash.Food;
        }
        else if (stash.Wood > 0)
        {
            text.text = "+" + stash.Wood;
        }
        else if (stash.Metal > 0)
        {
            text.text = "+" + stash.Metal;
        }
        else
        {
            background.SetActive(false);
        }
    }

    public void Collect()
    {
        gameController.Collect(Location.x, Location.y);
    }
}
