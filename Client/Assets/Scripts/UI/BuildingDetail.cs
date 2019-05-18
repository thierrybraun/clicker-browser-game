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
        public Text ResourceText;

        private Tile tile;

        public Tile Tile
        {
            get => tile;
            set
            {
                tile = value;
                if (tile.Building)
                {
                    Title.text = tile.Building.name;
                    ResourceIcon.sprite = tile.Building.CollectionSprite;
                    ResourceProgressbar.fillAmount = 0;
                    ResourceText.text = tile.GetTimeUntilNextUpdate().Seconds + "s";
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (tile)
            {
                ResourceText.text = tile.GetTimeUntilNextUpdate().Seconds + "s";
                ResourceProgressbar.fillAmount = tile.GetProgressUntilNextUpdate();
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
    }
}