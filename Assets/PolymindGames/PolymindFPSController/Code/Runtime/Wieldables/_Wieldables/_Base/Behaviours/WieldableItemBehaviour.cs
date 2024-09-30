using PolymindGames.InventorySystem;
using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    public class WieldableItemBehaviour : MonoBehaviour
    {
        public IWieldableItem Item { get; private set; }


        protected virtual void Awake()
        {
            Item = GetComponentInParent<IWieldableItem>();

            if (Item == null)
            {
                Debug.LogError($"No wieldable item found for {gameObject.name}.");
                return;
            }

            Item.AttachedItemChanged += OnItemChanged;
        }

        protected virtual void OnItemChanged(IItem item) { }
    }
}
