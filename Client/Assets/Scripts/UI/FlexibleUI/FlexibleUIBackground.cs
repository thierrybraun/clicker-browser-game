using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class FlexibleUIBackground : FlexibleUI
    {
        public TextType Type = TextType.Light;

        public enum TextType
        {
            Light, Dark
        }

        protected override void OnSkinUI()
        {
            var text = GetComponent<Image>();

            if (base.skinData)
            {
                switch (Type)
                {
                    case TextType.Light:
                        text.color = base.skinData.BackgroundLightColor;
                        break;
                    case TextType.Dark:
                        text.color = base.skinData.BackgroundDarkColor;
                        break;
                }
            }
        }
    }
}