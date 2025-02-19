using PolymindGames.ProceduralMotion;
using System;
using UnityEngine;

namespace PolymindGames.WieldableSystem.Effects
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class WieldableShakeEffect : WieldableEffect
    {
        [SerializeField]
        private ShakeSettings3D m_PositionShake = ShakeSettings3D.Default;

        [SerializeField]
        private ShakeSettings3D m_RotationShake = ShakeSettings3D.Default;

        private AdditiveShakeMotionModule m_ShakeMotion;
        
        
        public override void OnInitialized(IWieldable wieldable)
        {
            base.OnInitialized(wieldable);

            m_ShakeMotion = wieldable.gameObject.GetComponentInFirstChildren<FPWieldableMotionMixer>().GetMotionOfType<AdditiveShakeMotionModule>();
        }

        public override void PlayEffect()
        {
            m_ShakeMotion.AddPositionShake(m_PositionShake);
            m_ShakeMotion.AddRotationShake(m_RotationShake);
        }

        public override void PlayEffectDynamically(float value)
        {
            m_ShakeMotion.AddPositionShake(m_PositionShake, value);
            m_ShakeMotion.AddRotationShake(m_RotationShake, value);
        }
    }
}
