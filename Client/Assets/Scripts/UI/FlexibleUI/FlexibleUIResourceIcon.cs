using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class FlexibleUIResourceIcon : FlexibleUI
    {
        public enum ResourceType
        {
            Food, Wood, Metal
        }

        public ResourceType Type;        

        protected override void OnSkinUI()
        {
            var image = GetComponent<Image>();

            switch (Type)
            {
                case ResourceType.Food:
                    image.sprite = skinData.FoodSprite;
                    break;
                case ResourceType.Wood:
                    image.sprite = skinData.WoodSprite;
                    break;
                case ResourceType.Metal:
                    image.sprite = skinData.MetalSprite;
                    break;
            }
        }
    }
}