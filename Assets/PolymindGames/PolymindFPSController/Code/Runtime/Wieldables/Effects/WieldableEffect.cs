﻿using System;

namespace PolymindGames.WieldableSystem.Effects
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public abstract class WieldableEffect : IWieldableEffect
    {
#if UNITY_EDITOR
        public bool IsInitialized { get; protected set; }
#endif


        public virtual void OnInitialized(IWieldable wieldable)
        {
#if UNITY_EDITOR
            IsInitialized = true;
#endif
        }

        public abstract void PlayEffect();
        public virtual void PlayEffectDynamically(float value) => PlayEffect();
    }
}