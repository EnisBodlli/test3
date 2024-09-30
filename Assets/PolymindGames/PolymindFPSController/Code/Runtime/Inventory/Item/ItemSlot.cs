using System;
using UnityEngine;

namespace PolymindGames.InventorySystem
{
    /// <summary>
    /// Holds a reference to an item and listens to changes made to it.
    /// </summary>
    [Serializable]
	public sealed class ItemSlot
	{
		public struct CallbackContext
		{
			public readonly ItemSlot Slot;
			public readonly CallbackType Type;


			public CallbackContext(ItemSlot slot, CallbackType type)
			{
				this.Slot = slot;
				this.Type = type;
			}
        }
		
		public enum CallbackType
		{
			ItemAdded,
			ItemRemoved,
			StackChanged,
			PropertyChanged
		}
		
		public bool HasItem => m_Item != null;

		public IItem Item
		{
			get => m_Item;
			set
			{
				if (m_Item == value)
					return;
				
				// Stop listening for changes to the previously attached item.
				if (m_Item != null)
				{
					m_Item.PropertyChanged -= PropertyChanged;
					m_Item.StackCountChanged -= StackChanged;
				}

				m_Item = value;

				// Start listening for changes to the newly attached item.
				if (m_Item != null)
				{
					m_Item.PropertyChanged += PropertyChanged;
					m_Item.StackCountChanged += StackChanged;

					ItemChanged?.Invoke(new CallbackContext(this, CallbackType.ItemAdded));
				}
				else
					ItemChanged?.Invoke(new CallbackContext(this, CallbackType.ItemRemoved));

			}
		}

		public bool HasContainer => m_Container != null;

		public IItemContainer Container => m_Container;

		/// <summary> Sent when this slot has changed (e.g. when the attached item has changed).</summary>
		public event ItemSlotChangedDelegate ItemChanged;
		
		[OdinSerializer.OdinSerialize]
		private IItem m_Item;

		private IItemContainer m_Container;


		public ItemSlot(IItemContainer container)
		{
			this.m_Item = null;
			this.m_Container = container;
		}

		public ItemSlot(IItem item, IItemContainer container)
		{
			this.m_Item = item;
			this.m_Container = container;
		}
		
		public void Initialize(IItemContainer container)
		{
			if (m_Container != null)
			{
				Debug.LogError("Container already initialized.");
				return;
			}

			if (container == null)
			{
				Debug.LogError("Cannot initialize slot with a null container.");
				return;
			}

			// Stop listening for changes to the previously attached item.
			if (m_Item != null)
			{
				m_Item.PropertyChanged += PropertyChanged;
				m_Item.StackCountChanged += StackChanged;
			}

			m_Container = container;
		}
		
		private void StackChanged()
		{
			if (m_Item.StackCount == 0)
			{
				Item = null;
				return;
			}

			ItemChanged?.Invoke(new CallbackContext(this, CallbackType.StackChanged));
		}

		private void PropertyChanged() => ItemChanged?.Invoke(new CallbackContext(this, CallbackType.PropertyChanged));
	}

	public delegate void ItemSlotChangedDelegate(ItemSlot.CallbackContext context);
}
