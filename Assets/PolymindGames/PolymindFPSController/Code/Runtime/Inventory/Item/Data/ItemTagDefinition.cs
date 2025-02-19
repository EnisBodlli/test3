using UnityEngine;

namespace PolymindGames.InventorySystem
{
    [CreateAssetMenu(menuName = "Polymind Games/Items/Item Tag", fileName = "(Tag) ")]
    public sealed class ItemTagDefinition : DataDefinition<ItemTagDefinition>
    {
        public override string Name
        {
            get => m_TagName;
            protected set => m_TagName = value;
        }

        public override string FullName => m_TagName;

        [SerializeField, HideLabel]
        private string m_TagName;
    }
}