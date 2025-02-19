using System;
using UnityEngine;

namespace PolymindGames.ProceduralMotion
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class CurvesMotionData : MotionDataBase
    {
        public SpringSettings PositionSettings => m_PositionSpring;
        public SpringSettings RotationSettings => m_RotationSpring;
        public AnimCurves3D PositionCurves => m_PositionCurves; 
        public AnimCurves3D RotationCurves => m_RotationCurves;

        [Title("Position Settings")]

        [SerializeField]
        private SpringSettings m_PositionSpring = SpringSettings.Default;
        
        [SerializeField]
        private AnimCurves3D m_PositionCurves;
        
        [Title("Rotation Settings")]
        
        [SerializeField]
        private SpringSettings m_RotationSpring = SpringSettings.Default;

        [SerializeField]
        private AnimCurves3D m_RotationCurves;
    }
}