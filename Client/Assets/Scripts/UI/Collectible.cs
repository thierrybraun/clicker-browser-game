using UnityEngine;
using UnityEngine.UI;

namespace UI
{
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
        }

        private void Update()
        {
            if (!Tile) return;

            background.SetActive(false);
            var Stash = Tile.Stash;

            if (Stash.Food > 0)
            {
                text.text = "+" + LargeNumberFormat.FormatNoDecimal(Stash.Food);
                background.SetActive(true);
            }
            else if (Stash.Wood > 0)
            {
                text.text = "+" + LargeNumberFormat.FormatNoDecimal(Stash.Wood);
                background.SetActive(true);
            }
            else if (Stash.Metal > 0)
            {
                text.text = "+" + LargeNumberFormat.FormatNoDecimal(Stash.Metal);
                background.SetActive(true);
            }
        }

        private void LateUpdate()
        {
            if (Tile)
            {
                var p = Camera.main.WorldToScreenPoint(Tile.transform.position);
                transform.position = p;
            }
        }

        public void Collect()
        {
            API.API.Instance.CollectResources(GameState.Instance.CurrentCityId.Value, Tile.X, Tile.Y, OnCollected);
        }

        private void OnCollected(API.CollectResourcesResponse res)
        {
            Debug.Log("Collect: \n" + JsonUtility.ToJson(res, true));
            if (res.Success)
            {
                Tile.Stash = res.Resources.ToResourceCollection();
                if (GameState.Instance.CurrentCity != null)
                {
                    var city = GameState.Instance.CurrentCity.Value;
                    city.food = res.CityResources.Food;
                    city.wood = res.CityResources.Wood;
                    city.metal = res.CityResources.Metal;
                    GameState.Instance.CurrentCity = city;
                }
                background.SetActive(false);
            }
            else
            {
                Debug.LogError(res.Error);
            }
        }

    }
}