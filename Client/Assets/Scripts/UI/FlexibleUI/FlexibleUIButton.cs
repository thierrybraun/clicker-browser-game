using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlexibleUIButton : FlexibleUI
    {
        public enum ButtonType
        {
            Light, Dark, Danger
        }

        public ButtonType Type = ButtonType.Light;
        private Image background;
        private Text text;

        public override void Awake()
        {
            background = GetComponent<Image>();
            text = transform.Find("Text").GetComponent<Text>();
            base.Awake();
        }

        protected override void OnSkinUI()
        {
            if (base.skinData)
            {
                switch (Type)
                {
                    case ButtonType.Light:
                        text.color = base.skinData.TextDarkColor;
                        background.color = base.skinData.ButtonBackgroundLightColor;
                        break;
                    case ButtonType.Dark:
                        text.color = base.skinData.TextLightColor;
                        background.color = base.skinData.ButtonBackgroundDarkColor;
                        break;
                    case ButtonType.Danger:
                        text.color = base.skinData.ButtonTextDangerColor;
                        background.color = base.skinData.ButtonBackgroundDangerColor;
                        break;
                }
            }
        }
    }
}