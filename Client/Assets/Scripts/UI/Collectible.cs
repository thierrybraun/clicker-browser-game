using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Collectible : MonoBehaviour
{
    public Vector2Int Location;

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
        transform.localRotation = Quaternion.Euler(new Vector3(45f, 45f, 0f));
    }

    private void Update()
    {
        var stash = GameState.Instance.ResourceCollection.Where(s => s.X == Location.x && s.Y == Location.y).FirstOrDefault();
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
        GameState.Instance.Api.CollectResources(GameState.Instance.CurrentCityId.Value, Location.x, Location.y, OnCollected);        
    }

    private void OnCollected(API.CollectResourcesResponse res)
    {
        Debug.Log("Collect: \n" + JsonUtility.ToJson(res, true));
        var newStash = res.Resources;
        var oldStash = GameState.Instance.ResourceCollection.Where(s => s.X == newStash.X && s.Y == newStash.Y).First();
        GameState.Instance.ResourceCollection.Remove(oldStash);
        GameState.Instance.ResourceCollection.Add(newStash);
        GameState.Instance.MyPlayer = res.Player;
    }

}
