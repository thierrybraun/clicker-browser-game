using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class BuildingDetail : MonoBehaviour, IPointerClickHandler
    {
        public Text Title;
        public Image ResourceIcon;
        public Image ResourceProgressbar;

        private Text ResourceTimer;
        private Text ResourceAmount;
        private Text CurrentLevel, FoodCost, WoodCost, MetalCost;
        private Button UpgradeButton;
        private GameController GameController;

        public Vector2Int? TileCoordinates;

        public void OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            GameController = FindObjectOfType<GameController>();
            CurrentLevel = transform.Find("Card/Detail/InfoContainer/CurrentLevel").GetComponent<Text>();
            FoodCost = transform.Find("Card/Detail/InfoContainer/FoodCost").GetComponent<Text>();
            WoodCost = transform.Find("Card/Detail/InfoContainer/WoodCost").GetComponent<Text>();
            MetalCost = transform.Find("Card/Detail/InfoContainer/MetalCost").GetComponent<Text>();
            UpgradeButton = transform.Find("Card/UpgradeButton").GetComponent<Button>();
            ResourceTimer = transform.Find("Card/Detail/ProductionContainer/Timer").GetComponent<Text>();
            ResourceAmount = transform.Find("Card/Detail/ProductionContainer/Amount").GetComponent<Text>();
        }

        private void Update()
        {
            if (!TileCoordinates.HasValue) return;
            var tile = GameController.GetTile(TileCoordinates.Value.x, TileCoordinates.Value.y);
            if (tile && tile.Building)
            {
                Title.text = tile.Building.name;
                ResourceIcon.sprite = tile.Building.CollectionSprite;
                ResourceTimer.text = tile.GetTimeUntilNextUpdate().Seconds + "s";

                CurrentLevel.text = "Level: " + tile.BuildingLevel;

                var cost = tile.Building.BuildCostFunction.GetCost(tile.BuildingLevel + 1);
                FoodCost.text = "Food: " + cost.Food;
                WoodCost.text = "Wood: " + cost.Wood;
                MetalCost.text = "Metal: " + cost.Metal;

                var production = tile.Building.ProductionFunction.GetProduction(tile.BuildingLevel);
                ResourceAmount.text = "+" + (production.Food + production.Metal + production.Wood);

                var player = GameState.Instance.MyPlayer;
                UpgradeButton.enabled = player.Wood >= cost.Wood && player.Metal >= cost.Metal && player.Food >= cost.Food;
            }
        }

        private void LateUpdate()
        {
            if (!TileCoordinates.HasValue) return;
            var tile = GameController.GetTile(TileCoordinates.Value.x, TileCoordinates.Value.y);
            if (tile && tile.Building)
            {
                ResourceProgressbar.fillAmount = tile.GetProgressUntilNextUpdate();
            }
        }

        public void Upgrade()
        {
            if (!TileCoordinates.HasValue) return;
            var tile = GameController.GetTile(TileCoordinates.Value.x, TileCoordinates.Value.y);
            FindObjectOfType<GameController>().UpgradeBuilding(tile);
        }
    }
}