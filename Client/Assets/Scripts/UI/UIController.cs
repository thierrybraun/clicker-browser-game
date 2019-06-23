using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        public GameObject CollectiblePrefab;
        private BuildingDetail BuildingDetail;
        private Text Food, Wood, Metal;
        private BuildMenu BuildMenu;
        private Transform CollectibleContainer;
        private IDictionary<Vector2Int, Collectible> RegisteredCollectibles = new Dictionary<Vector2Int, Collectible>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            Food = transform.Find("Resources/Food/Text").GetComponent<Text>();
            Wood = transform.Find("Resources/Wood/Text").GetComponent<Text>();
            Metal = transform.Find("Resources/Metal/Text").GetComponent<Text>();
            BuildMenu = transform.Find("Buildmenu").GetComponent<BuildMenu>();
            BuildingDetail = transform.Find("BuildingDetail").GetComponent<BuildingDetail>();
            CollectibleContainer = transform.Find("CollectibleContainer");
        }

        private void Update()
        {
            var State = GameState.Instance;
            if (State.CurrentCity.HasValue)
            {
                Food.text = LargeNumberFormat.FormatTwoDecimal(State.CurrentCity.Value.currency.Food);
                Wood.text = LargeNumberFormat.FormatTwoDecimal(State.CurrentCity.Value.currency.Wood);
                Metal.text = LargeNumberFormat.FormatTwoDecimal(State.CurrentCity.Value.currency.Metal);
            }

            var distances = new List<Tuple<Transform, float>>();
            foreach (Transform t in CollectibleContainer)
            {
                distances.Add(Tuple.Create(t, Vector3.Distance(Camera.main.transform.position, t.position)));
            }
            distances.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            for (int i = 0; i < distances.Count; i++)
            {
                distances[i].Item1.SetSiblingIndex(i);
            }
        }

        public void ShowBuilding(Tile tile)
        {
            HideMenues();
            if (tile.Building)
            {
                BuildingDetail.TileCoordinates = new Vector2Int(tile.X, tile.Y);
                BuildingDetail.gameObject.SetActive(true);
            }
        }

        public void HideMenues()
        {
            BuildMenu.gameObject.SetActive(false);
            BuildingDetail.gameObject.SetActive(false);
        }

        public void ShowBuildMenu(Tile tile)
        {
            HideMenues();
            BuildMenu.Tile = tile;
            BuildMenu.gameObject.SetActive(true);
        }

        public void Register(Tile tile)
        {
            Sprite sprite = tile.Building?.CollectionSprite;

            if (sprite)
            {
                Collectible collectible = Instantiate(CollectiblePrefab, CollectibleContainer).GetComponent<Collectible>();
                collectible.name = "CollectionPopup_" + tile.X + "_" + tile.Y;
                collectible.Sprite = sprite;
                collectible.Tile = tile;

                var key = new Vector2Int(tile.X, tile.Y);
                if (RegisteredCollectibles.ContainsKey(key))
                {
                    DestroyImmediate(RegisteredCollectibles[key].gameObject);
                    RegisteredCollectibles.Remove(key);
                }
                RegisteredCollectibles.Add(key, collectible);
            }
        }
    }
}