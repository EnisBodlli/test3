using PolymindGames.InventorySystem;

namespace PolymindGames
{
    public interface IItemDropHandler : ICharacterModule
    {
        void DropItem(IItem itemToDrop, float dropDelay = 0f);
        void DropItem(ItemSlot itemSlot, float dropDelay = 0f);
    }
}