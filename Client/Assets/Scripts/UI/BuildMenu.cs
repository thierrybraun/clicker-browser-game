using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildMenu : MonoBehaviour
    {
        public Text Title;
        public Transform Food, Wood, Metal;
        public Button Confirm;

        private Tile tile;

        public Tile Tile
        {
            get => tile;
            set
            {
                tile = value;
                transform.position = Tile.transform.position + Vector3.up * 10;

                if (tile.Resource)
                {
                    Title.text = "Build " + tile.Resource.Building.name;
                    var cost = tile.Resource.Building.BuildCostFunction.GetCost(1);
                    Food.GetComponentInChildren<Text>().text = cost.Food.ToString();
                    Wood.GetComponentInChildren<Text>().text = cost.Wood.ToString();
                    Metal.GetComponentInChildren<Text>().text = cost.Metal.ToString();
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (Tile && Tile.Resource)
            {
                var cost = Tile.Resource.Building.BuildCostFunction.GetCost(1);
                var city = GameState.Instance.CurrentCity;
                if (city == null) return;
                var player = GameState.Instance.MyPlayer;
                Confirm.enabled = city.Value.wood >= cost.Wood && city.Value.metal >= cost.Metal && city.Value.food >= cost.Food;

                var correctColor = new Color(0.1960784f, 0.1960784f, 0.1960784f);
                var missingColor = new Color(1f, 0f, 1f);
                Food.GetComponentInChildren<Text>().color = city.Value.food >= cost.Food ? correctColor : missingColor;
                Wood.GetComponentInChildren<Text>().color = city.Value.wood >= cost.Wood ? correctColor : missingColor;
                Metal.GetComponentInChildren<Text>().color = city.Value.metal >= cost.Metal ? correctColor : missingColor;
            }

            if (Input.GetMouseButton(1))
            {
                gameObject.SetActive(false);
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

        public void Build()
        {
            Building building = Tile.Resource.Building;
            FindObjectOfType<GameController>().Build(building, Tile.X, Tile.Y);
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
        }
    }
}