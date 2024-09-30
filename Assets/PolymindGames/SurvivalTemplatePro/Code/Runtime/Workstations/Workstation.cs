using Mirror;
using PolymindGames.BuildingSystem;
using PolymindGames.InventorySystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames
{
    public class Workstation : Interactable, IWorkstation
    {
        public string WorkstationName => Title;
        public bool InspectionActive { get; private set; }

        [SerializeField]
        private SoundPlayer m_OpenSound;

        [SpaceArea]

        [SerializeField]
        private UnityEvent m_OpenStationEvent;

        [SpaceArea]

        [SerializeField]
        private SoundPlayer m_CloseSound;

        [SpaceArea]

        [SerializeField]
        private UnityEvent m_CloseStationEvent;
        
        [SyncVar] protected bool m_IsBeingUsed = false;

        public bool InUse => m_IsBeingUsed;
        public virtual IItemContainer[] GetContainers() => Array.Empty<IItemContainer>();

        public sealed override void OnInteract(Character character)
        {
            base.OnInteract(character);

            if (character.TryGetModule(out IInventoryInspectManager inspection))
                inspection.StartInspection(this, OnInspectionStart, OnInspectionEnd);
        }

        protected virtual void OnInspectionStart()
        {
            m_OpenStationEvent.Invoke();
            m_OpenSound.Play2D();
            InspectionActive = true;
        }

        protected virtual void OnInspectionEnd()
        {
            m_CloseStationEvent.Invoke();
            m_CloseSound.Play2D();
            InspectionActive = false;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            UnityUtils.SafeOnValidate(this, () =>
            {
                if (TryGetComponent(out Buildable buildable))
                {
                    Title = buildable.Definition.Name;
                    Description = buildable.Definition.Description;
                }
            });
        }
#endif
    }
}
