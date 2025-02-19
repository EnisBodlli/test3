﻿using System;
using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    public class WieldableArmsHandler : MonoBehaviour, IWieldableArmsHandler
    {
        #region Internal
        [Serializable]
        private struct ArmSet
        {
            public string Name;

            [Space(3f)]

            public SkinnedMeshRenderer LeftArm;
            public SkinnedMeshRenderer RightArm;
        }
        #endregion

        public Animator Animator => m_Animator;

        [SerializeField, NotNull]
        private Animator m_Animator;

        [SerializeField, NotNull]
        private ChildOfConstraint m_ChildConstraint;

        [SpaceArea]

        [SerializeField]
        private SkinnedMeshRenderer m_LeftArm;

        [SerializeField]
        private SkinnedMeshRenderer m_RightArm;

        [SpaceArea]

        [SerializeField, ReorderableList]
        private ArmSet[] m_ArmSets;

        private int m_SelectedArmsIndex = -1;
        private Animator m_TargetAnimator;


        public void EnableArms(bool enable, float delay = 0)
        {
            if (delay < 0.001f)
                gameObject.SetActive(enable);
            else
                UpdateManager.InvokeDelayedAction(this, DisableArms, delay);

            void DisableArms() => gameObject.SetActive(enable);
        }

        public void ToggleNextArmSet() 
        {
            var arms = m_ArmSets.Select(ref m_SelectedArmsIndex, SelectionType.Sequence);

            m_LeftArm.sharedMesh = arms.LeftArm.sharedMesh;
            m_LeftArm.sharedMaterials = arms.LeftArm.sharedMaterials;

            m_RightArm.sharedMesh = arms.RightArm.sharedMesh;
            m_RightArm.sharedMaterials = arms.RightArm.sharedMaterials;
        }

        public void SyncWithAnimator(Animator animator) => m_TargetAnimator = animator;
        public void SetParent(Transform parent) => m_ChildConstraint.Parent = parent;

        private void Update()
        {
            if (m_Animator.runtimeAnimatorController == null || m_TargetAnimator == null)
                return;

            for (int i = 0; i < m_Animator.layerCount; i++)
            {
                var stateInfo = m_TargetAnimator.GetCurrentAnimatorStateInfo(i);
                m_Animator.Play(stateInfo.fullPathHash, i, stateInfo.normalizedTime);
                m_Animator.SetLayerWeight(i, m_TargetAnimator.GetLayerWeight(i));
            }
        }

        private void Start()
        {
            ToggleNextArmSet();
        }
    }
}
