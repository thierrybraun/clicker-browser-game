using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class FlexibleUIText : FlexibleUI
    {
        public TextType Type = TextType.Light;

        public enum TextType
        {
            Light, Dark
        }

        protected override void OnSkinUI()
        {
            var text = GetComponent<Text>();

            if (base.skinData)
            {
                switch (Type)
                {
                    case TextType.Light:
                        text.color = base.skinData.TextLightColor;
                        break;
                    case TextType.Dark:
                        text.color = base.skinData.TextDarkColor;
                        break;
                }
            }
        }
    }
}