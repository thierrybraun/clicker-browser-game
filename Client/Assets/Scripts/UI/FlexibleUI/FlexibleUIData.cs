﻿using UnityEngine;

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
    }
}