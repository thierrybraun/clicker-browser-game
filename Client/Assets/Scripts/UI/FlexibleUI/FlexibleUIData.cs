using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "Flexible UI Data")]
    public class FlexibleUIData : ScriptableObject
    {
        [Header("Text Color")]
        public Color TextLightColor;
        public Color TextDarkColor;

        [Header("Background Color")]
        public Color BackgroundLightColor;
        public Color BackgroundDarkColor;

        [Header("Icons")]
        public Sprite FoodSprite;
        public Sprite WoodSprite;
        public Sprite MetalSprite;        

        [Header("Button")]
        public Color ButtonBackgroundLightColor;
        public Color ButtonBackgroundDarkColor;
        public Color ToggleButtonSelectedLightColor;
        public Color ToggleButtonSelectedDarkColor;
        public Color ButtonTextDangerColor;
        public Color ButtonBackgroundDangerColor;
    }    
}