using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlexibleUIToggleButton : FlexibleUI
    {
        public enum ToggleType
        {
            Light, Dark
        }

        public ToggleType Type = ToggleType.Light;
        private Image background, checkmark;
        private Text text;

        public override void Awake()
        {
            background = transform.Find("Background").GetComponent<Image>();
            checkmark = transform.Find("Background/Checkmark").GetComponent<Image>();
            text = transform.Find("Label").GetComponent<Text>();
            base.Awake();
        }

        protected override void OnSkinUI()
        {
            if (base.skinData)
            {
                switch (Type)
                {
                    case ToggleType.Light:
                        text.color = base.skinData.TextDarkColor;
                        background.color = base.skinData.ButtonBackgroundLightColor;
                        checkmark.color = base.skinData.ToggleButtonSelectedLightColor;
                        break;
                    case ToggleType.Dark:
                        text.color = base.skinData.TextLightColor;
                        background.color = base.skinData.ButtonBackgroundDarkColor;
                        checkmark.color = base.skinData.ToggleButtonSelectedDarkColor;
                        break;
                }
            }
        }
    }
}