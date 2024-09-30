using System;
using System.Collections.Generic;
using UnityEngine;

namespace PolymindGames.BuildingSystem
{
    public class StructureBuildable : Buildable
    {
        public StructureManager ParentStructure { get; set; }

        public DataIdReference<BuildableCategoryDefinition> RequiredSpace => m_RequiredSpace;
        public DataIdReference<BuildableCategoryDefinition>[] SpacesToOccupy => m_SpacesToOccupy;

		public bool RequiresSockets => m_RequiresSockets;
		public Socket[] Sockets => m_Sockets;
        public Vector3 OccupiedSocketPosition 
		{
			get => m_OccupiedSocketPosition;
			set => m_OccupiedSocketPosition = value;
		}

		[Title("Settings (Sockets)")]

		[SerializeField]
		private bool m_RequiresSockets = true;

		[SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<BuildableCategoryDefinition> m_RequiredSpace;

		[SpaceArea]

		[SerializeField]
		[DataReferenceDetails(HasNullElement = false)]
        [ReorderableList(HasLabels = false, Foldable = true)]
        private DataIdReference<BuildableCategoryDefinition>[] m_SpacesToOccupy;

        [SerializeField]
        private NewSocket[] _sockets;

		private Vector3 m_OccupiedSocketPosition;
        private Socket[] m_Sockets;


		public override void OnCreated(bool playEffects = true)
		{
			EnableColliders(false);
			EnableSockets(false);

			gameObject.SetLayerRecursively(BuildingManager.BuildablePreviewLayer);
			MaterialEffect.EnableCustomEffect(BuildingManager.PlacementAllowedMaterialEffect);
		}

		public override void OnPlaced(bool playEffects = true)
		{
			EnableSockets(true);

			if (playEffects)
				DoPlacementEffects();

			EnableColliders(true);
		}

		public override void OnBuilt(bool playEffects = true)
        {
			gameObject.SetLayerRecursively(BuildingManager.BuildableLayer);
			MaterialEffect.DisableActiveEffect();

			EnableSockets(true);

			if (playEffects)
				DoBuildEffects();
		}

		protected void EnableSockets(bool enable)
		{
			foreach (var socket in m_Sockets)
				socket.gameObject.SetActive(enable);
		}

		protected override void Awake()
		{
			base.Awake();

			m_Sockets = gameObject.GetComponentsInFirstChildren<Socket>().ToArray();
		}
		
        private void OnValidate()
        {
            if (_sockets != null && _sockets.Length == 0)
            {
                var sockets = gameObject.GetComponentsInFirstChildren<Socket>();
                _sockets = new NewSocket[sockets.Count];
                for (int i = 0; i < sockets.Count; i++)
                {
                    var socket = sockets[i];
                    Vector3 offset = socket.transform.localPosition;
            
                    _sockets[i] = new NewSocket(socket.Offsets, offset);
                }
            }
        }
	}
    
    [Serializable]
    public sealed class NewSocket
    {
        [SerializeField]
        private Vector3 _offset;

        [SerializeField, SpaceArea]
        [ReorderableList(ListStyle.Lined)]
        private BuildableOffset[] _offsets;

        public NewSocket(Socket.BuildableOffset[] offsets, Vector3 offset)
        {
	        _offset = offset;

	        _offsets = new BuildableOffset[offsets.Length];

	        for (int i = 0; i < _offsets.Length; i++)
	        {
		        _offsets[i] = new BuildableOffset()
		        {
			        Category = offsets[i].Category,
			        PositionOffset = offsets[i].PositionOffset,
			        RotationOffset = offsets[i].RotationOffsetEuler
		        };
	        }
        }

        #region Internal
        [Serializable]
        public sealed class BuildableOffset
        {
            [SerializeField, DataReferenceDetails(HasLabel = false, HasNullElement = false)]
            public DataIdReference<BuildableCategoryDefinition> Category;

            [SerializeField]
            public Vector3 PositionOffset;

            [SerializeField]
            public Vector3 RotationOffset;
        }
        #endregion
    }
}
