using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Collectible : MonoBehaviour
{
    public Text text;
    public Image icon;
    public GameObject background;
    public Tile Tile;

    public Sprite Sprite
    {
        set
        {
            icon.sprite = value;
        }
    }

    private void Start()
    {
        background.SetActive(false);
        transform.localRotation = Quaternion.Euler(new Vector3(45f, 45f, 0f));
        transform.localScale = Vector3.one / 10f;
    }

    private void Update()
    {
        if (!Tile) return;
        transform.position = new Vector3(Tile.X * 10, 0, Tile.Y * 10);

        background.SetActive(false);
        var Stash = Tile.Stash;

        if (Stash.Food > 0)
        {
            text.text = "+" + Stash.Food;
            background.SetActive(true);
        }
        else if (Stash.Wood > 0)
        {
            text.text = "+" + Stash.Wood;
            background.SetActive(true);
        }
        else if (Stash.Metal > 0)
        {
            text.text = "+" + Stash.Metal;
            background.SetActive(true);
        }
    }

    public void Collect()
    {
        GameState.Instance.Api.CollectResources(GameState.Instance.CurrentCityId.Value, Tile.X, Tile.Y, OnCollected);
    }

    private void OnCollected(API.CollectResourcesResponse res)
    {
        Debug.Log("Collect: \n" + JsonUtility.ToJson(res, true));
        if (res.Success)
        {
            Tile.Stash = res.Resources.ToResourceCollection();
            GameState.Instance.MyPlayer = res.Player;
            background.SetActive(false);
        }
        else
        {
            Debug.LogError(res.Error);
        }
    }

}
