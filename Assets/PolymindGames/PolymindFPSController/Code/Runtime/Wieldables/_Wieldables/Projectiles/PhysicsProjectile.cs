using PolymindGames.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames.WieldableSystem
{
    public class PhysicsProjectile : PhysicsProjectileBase
    {
        private class CharacterEvent : UnityEvent<ICharacter> { }

        [SpaceArea]

        [SerializeField]
        private float m_DetonateDelay;

        [SerializeField]
        private UnityEvent m_DetonateEvent;

        [SpaceArea]

        [SerializeField]
        private ItemPickupBase m_ItemPickup;


        public override void LinkItem(IItem item)
        {
            if (m_ItemPickup != null)
                m_ItemPickup.LinkWithItem(item);
        }

        protected override void OnHit(Collision hit)
        {
            m_DetonateEvent.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_ItemPickup == null)
                m_ItemPickup = GetComponent<ItemPickupBase>();
        }
#endif
    }
}
