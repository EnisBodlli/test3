using PolymindGames.ProceduralMotion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [AddComponentMenu("PolymindGames/Wieldables/FP Motion")]
    public class FPWieldableMotionMixer : MotionMixer
    {
        public IMotionDataHandler DataHandler { get; set; }
        public MotionPreset MotionPreset => m_MotionPreset;

        [Title("Data")]

        [SerializeField]
        private MotionPreset m_MotionPreset;


        public void SetMotions(List<IMixedMotion> motions, Dictionary<Type, IMixedMotion> motionsDict)
        {
            m_Motions = motions ?? s_EmptyList;
            m_MotionsDict = motionsDict;
        }
    }
}