using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace PolymindGames.InventorySystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/inventory#item-drop-handler-module")]
	public sealed class ItemDropHandler : NetworkCharacterBehaviour, IItemDropHandler
	{
		[SerializeField]
		[Tooltip("The layer mask that will be used in checking for obstacles when items are dropped.")]
		private LayerMask m_DropObstacleMask = Physics.DefaultRaycastLayers;

		[SerializeField, FormerlySerializedAs("m_ItemDropSettings")]
		[Tooltip("Position, rotation offsets etc.")]
		private ObjectDropper.DropSettings m_DropSettings;

		[SerializeField]
		private StandardSound m_DropSound;

		[Line]

		[SerializeField, AssetPreview]
		[Tooltip("The prefab used when an item that's being dropped doesn't have a pickup (e.g. clothes) or when dropping multiple items at once.")]
		private GameObject m_GenericPrefab;

		private ObjectDropper m_ItemDropper;


        protected override void OnBehaviourEnabled()
        {
			m_ItemDropper = new ObjectDropper(Character, Character.ViewTransform, m_DropObstacleMask);
		}

        public void DropItem(IItem itemToDrop, float dropDelay = 0f)
		{
			if (itemToDrop == null || !IsBehaviourEnabled)
				return;

			StartItemDrop(itemToDrop, dropDelay);

			// Remove dropped item from the inventory
			Character.Inventory.RemoveItem(itemToDrop);
		}
		
		public void DropItem(ItemSlot itemSlot, float dropDelay = 0)
		{
			if (itemSlot == null || !itemSlot.HasItem || !IsBehaviourEnabled)
				return;

			StartItemDrop(itemSlot.Item, dropDelay);

			// Remove dropped item from the slot
			itemSlot.Item = null;
		}
		
		private void StartItemDrop(IItem itemToDrop, float dropDelay)
		{
			if (dropDelay > 0.01f)
				StartCoroutine(C_DropWithDelay(itemToDrop, dropDelay));
			else
				DropItem(itemToDrop);
		}
		
		private void DropItem(IItem itemToDrop) 
		{
			if (itemToDrop == null)
				return;

			GameObject prefabToDrop;

			if (itemToDrop.StackCount == 1 && itemToDrop.Definition.Pickup != null)
				prefabToDrop = itemToDrop.Definition.Pickup.gameObject;
			else
				prefabToDrop = m_GenericPrefab;

			float dropHeightMod = Character.GetModule<IMovementController>().ActiveState == MovementStateType.Crouch ? 0.5f : 1f;
			//GameObject droppedObj = m_ItemDropper.DropObject(m_DropSettings, prefabToDrop, dropHeightMod);

			DropItemCmd(itemToDrop.Definition.Id, dropHeightMod, itemToDrop.StackCount);

            Character.AudioPlayer.PlaySound(m_DropSound);

			// Link the pickup with the dropped object
			/*if (droppedObj.TryGetComponent(out ItemPickup itemPickup))
				itemPickup.LinkWithItem(itemToDrop);*/
		}

		[Command]

		private void DropItemCmd(int id, float dropHeightMod, int stackCount)
        {

            ItemDefinition def = ItemDefinition.GetWithId(id);

            if (def == null)
            {
				return;
            }
            GameObject prefabToDrop = null;

            if (stackCount == 1 && def.Pickup != null)
                prefabToDrop = def.Pickup.gameObject;
            else
                prefabToDrop = m_GenericPrefab;

            Debug.Log("Drop Item cmd" + prefabToDrop);
            Debug.Log(m_ItemDropper);
            if (prefabToDrop == null) return;

            GameObject droppedObj = m_ItemDropper.DropObject(m_DropSettings, prefabToDrop, dropHeightMod);

			NetworkServer.Spawn(droppedObj);

			LinkItemRPC(droppedObj, id, stackCount);

        }
		[ClientRpc]
		public void LinkItemRPC(GameObject droppedObj, int itemid, int stack)
		{
			ItemDefinition def = ItemDefinition.GetWithId(itemid);

			Item item = new Item(def,stack);

				if (droppedObj.TryGetComponent(out ItemPickup itemPickup))
				itemPickup.LinkWithItem(item);
		}

        private IEnumerator C_DropWithDelay(IItem itemToDrop, float dropDelay) 
		{
			yield return new WaitForSeconds(dropDelay);

			DropItem(itemToDrop);
		}
    }
}