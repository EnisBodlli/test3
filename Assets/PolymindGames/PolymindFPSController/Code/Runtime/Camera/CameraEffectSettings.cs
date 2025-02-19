﻿using UnityEngine;
using System;

namespace PolymindGames
{
    [Serializable]
    public sealed class CameraEffectSettings
    {
        #region Internal
        [Serializable]
        public class Effect
        {
            public bool Enabled;

            [Range(-100f, 100f)]
            public float ValueChange = 0f;

            public AnimationCurve ValueChangeOverTime = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        }
        #endregion

        [Range(0.1f, 15f)]
        public float Duration = 1f;

        public Effect ExposureEffect;
        public Effect SaturationEffect;
        public Effect VignetteEffect;
        public Effect ChromaticAberationEffect;
    }
}