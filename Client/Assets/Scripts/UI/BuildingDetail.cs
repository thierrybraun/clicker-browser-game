using System.Linq;
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
        private ToggleGroup upgradeAmount;
        private Text nextLevel;
        private Text CurrentLevel, FoodCost, WoodCost, MetalCost;
        private Button UpgradeButton;
        private GameController GameController;
        private int targetLevel;
        private Currency upgradeCost;

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
            upgradeAmount = transform.Find("Card/UpgradeAmount").GetComponent<ToggleGroup>();
            nextLevel = transform.Find("Card/Detail/InfoContainer/CostForNextLevel").GetComponent<Text>();
            nextLevel.text = "Upgrade to level: ";

            RecalculateTargetLevel();
        }

        public void RecalculateTargetLevel()
        {
            var selected = "One";
            if (upgradeAmount.AnyTogglesOn())
            {
                selected = upgradeAmount.ActiveToggles().First().name;
            }

            var tile = GetCurrentTile();
            if (tile == null) return;
            var current = tile.BuildingLevel;
            var max = tile.Building.BuildCostFunction.GetMaxLevel(current, GameState.Instance.CurrentCity?.Currency ?? new Currency());
            if (max < current) return;
            switch (selected)
            {
                case "One":
                    targetLevel = tile.BuildingLevel + 1;
                    break;
                case "10":
                    targetLevel = Mathf.Max(current + 1, (int)(max * 0.1f));
                    break;
                case "50":
                    targetLevel = Mathf.Max(current + 1, (int)(max * 0.5f));
                    break;
                case "Max":
                    targetLevel = Mathf.Max(current + 1, max);
                    break;
            }
            upgradeCost = tile.Building.BuildCostFunction.GetCostRange(current, targetLevel);
        }

        private Tile GetCurrentTile()
        {
            if (!TileCoordinates.HasValue) return null;
            var tile = GameController.GetTile(TileCoordinates.Value.x, TileCoordinates.Value.y);
            return tile;
        }
        private void Update()
        {
            var tile = GetCurrentTile();
            if (tile && tile.Building)
            {
                Title.text = tile.Building.name;
                ResourceIcon.sprite = tile.Building.CollectionSprite;
                ResourceTimer.text = tile.GetTimeUntilNextUpdate().Seconds + "s";

                CurrentLevel.text = "Level: " + tile.BuildingLevel;

                FoodCost.text = "Food: " + upgradeCost.Food;
                WoodCost.text = "Wood: " + upgradeCost.Wood;
                MetalCost.text = "Metal: " + upgradeCost.Metal;

                var production = tile.Building.ProductionFunction.GetProduction(tile.BuildingLevel);
                ResourceAmount.text = "+" + (production.Food + production.Metal + production.Wood);

                nextLevel.text = "Upgrade to level: " + targetLevel.ToString();

                var city = GameState.Instance.CurrentCity;
                if (city.HasValue)
                {
                    UpgradeButton.interactable = city?.Currency >= upgradeCost && targetLevel > tile.BuildingLevel;
                }
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

            var state = GameState.Instance;
            var city = state.CurrentCity.Value;
            city.Currency -= upgradeCost;
            tile.BuildingLevel = targetLevel;

            FindObjectOfType<GameController>().UpgradeBuilding(tile, targetLevel);
        }
    }
}