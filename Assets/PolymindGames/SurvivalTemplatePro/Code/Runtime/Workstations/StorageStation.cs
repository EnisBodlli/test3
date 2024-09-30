using Mirror;
using PolymindGames.InventorySystem;
using System.Collections.Generic;
using UnityEngine;

namespace PolymindGames
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/interaction/interactable/demo-interactables")]
    public sealed class StorageStation : Workstation, ISaveableComponent
    {
        public ItemContainer ItemContainer => m_ItemContainer;

        [SerializeField, Range(0, 100)]
        [Tooltip("How many slots should this storage crate have.")]
        private int m_StorageSpots;

        [SpaceArea]

        [SerializeField, ReorderableList(Foldable = true)]
        private ItemGenerator[] m_InitialItems;

        [SerializeField, Tooltip("Can a character add items to this storage.")]
        private bool m_CanAddItems = true;

        [SerializeField, Tooltip("Can a character remove items from this storage.")]
        private bool m_CanRemoveItems = true;

        private ItemContainer m_ItemContainer;

        public readonly SyncList<ItemNetworkedInfo> m_NetworkedItems = new SyncList<ItemNetworkedInfo>();
        public override IItemContainer[] GetContainers() => new IItemContainer[] { m_ItemContainer };


        private void Start()
        {
            m_ItemContainer = null;

            if (m_ItemContainer == null)
                GenerateContainer();

            m_InitialItems = null;
        }

        private void GenerateContainer()
        {

            m_ItemContainer = new ItemContainer("Storage", m_StorageSpots, GetContainerRestrictions());

            foreach (var itemGenerator in m_InitialItems)
                m_ItemContainer.AddItem(itemGenerator.GenerateItem());

            SyncModifiedData();
        }

        private ContainerRestriction[] GetContainerRestrictions()
        {
            var restrictions = new List<ContainerRestriction>();
            
            if (!m_CanAddItems)
                restrictions.Add(new ContainerAddRestriction());
            
            if (!m_CanRemoveItems)
                restrictions.Add(new ContainerRemoveRestriction());

            return restrictions.ToArray();
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            Debug.Log("Container was loaded");
            m_ItemContainer = members[0] as ItemContainer;
            m_ItemContainer.OnLoad();
        }
        protected override void OnInspectionEnd()
        {
            base.OnInspectionEnd();

            SyncModifiedData();
        }

        protected override void OnInspectionStart()
        {
            SetInUseStateCommand(true);

            m_ItemContainer.RemoveAllItems();

            foreach (var item in m_NetworkedItems)
            {
                m_ItemContainer.AddItem(new Item(ItemDefinition.GetWithId(item.ItemId), item.Count));
            }
            base.OnInspectionStart();


        }

        private void SyncModifiedData()
        {
            List<ItemNetworkedInfo> m_SyncedItems = new List<ItemNetworkedInfo>();

            foreach(ItemSlot t in m_ItemContainer.Slots)
            {
                if (t.HasItem)
                {
                    m_SyncedItems.Add(new ItemNetworkedInfo { ItemId = t.Item.Id , Count = t.Item.StackCount });
                }
            }

            if (m_SyncedItems == null) return;

            UpdateContainerCommand(m_SyncedItems);
            UpdateContainer(m_SyncedItems);
        }

        [Command(requiresAuthority = false)]

        private void UpdateContainerCommand(List<ItemNetworkedInfo> updatedItems)
        {
            UpdateContainer(updatedItems);
        }
        private void UpdateContainer(List<ItemNetworkedInfo> updatedItems)
        {
            if (isServer == false) return;

            if (m_NetworkedItems.Count > 0)
            {
                for (int i = m_NetworkedItems.Count - 1; i >= 0; i--)
                {
                    m_NetworkedItems.RemoveAt(i);
                }
            }


            foreach (ItemNetworkedInfo t in updatedItems)
            {
                m_NetworkedItems.Add(t);
            }
            m_IsBeingUsed = false;

            Debug.Log("Container now has " + m_NetworkedItems.Count);
        }


        [Command(requiresAuthority = false)]
        private void SetInUseStateCommand(bool value)
        {
            m_IsBeingUsed = value;
        }

        public object[] SaveMembers()
        {
            object[] members = new object[]
            {
                m_ItemContainer
            };

            return members;
        }
        #endregion
    }
}

[System.Serializable]
public struct ItemNetworkedInfo
{
    public int ItemId;
    public int Count;
}