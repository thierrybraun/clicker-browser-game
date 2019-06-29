using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlexibleUIDropdown : FlexibleUI
    {
        public enum DropdownType
        {
            Light, Dark
        }

        public DropdownType Type = DropdownType.Light;
        private Image background;
        private Text label;
        private Transform itemContainer;
        private Image dropdownList;

        public override void Awake()
        {
            background = GetComponent<Image>();
            label = transform.Find("Label").GetComponent<Text>();
            // checkmark = transform.Find("Background/Checkmark").GetComponent<Image>();
            // text = transform.Find("Label").GetComponent<Text>();
            base.Awake();
        }

        protected override void OnSkinUI()
        {
            var list = transform.Find("Dropdown List");
            if (list != null)
            {
                dropdownList = list.GetComponent<Image>();
                itemContainer = list.transform.Find("Viewport/Content");
            }

            if (base.skinData)
            {
                switch (Type)
                {
                    case DropdownType.Light:
                        background.color = base.skinData.ButtonBackgroundLightColor;
                        label.color = base.skinData.TextDarkColor;
                        if (dropdownList != null) dropdownList.color = base.skinData.ButtonBackgroundLightColor;
                        SetItemColors(base.skinData.TextDarkColor, base.skinData.ButtonBackgroundLightColor);
                        break;
                    case DropdownType.Dark:
                        background.color = base.skinData.ButtonBackgroundDarkColor;
                        label.color = base.skinData.TextLightColor;
                        if (dropdownList != null) dropdownList.color = base.skinData.ButtonBackgroundDarkColor;
                        SetItemColors(base.skinData.TextLightColor, base.skinData.ButtonBackgroundDarkColor);
                        break;
                }
            }
        }

        private void SetItemColors(Color text, Color bg)
        {
            if (itemContainer == null) return;
            foreach (Transform item in itemContainer)
            {
                item.Find("Item Label").GetComponent<Text>().color = text;
                item.Find("Item Background").GetComponent<Image>().color = bg;
            }
        }
    }
}