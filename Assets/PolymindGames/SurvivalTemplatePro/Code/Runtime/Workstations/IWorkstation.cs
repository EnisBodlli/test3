using PolymindGames.InventorySystem;

namespace PolymindGames
{
    public interface IWorkstation
    {
        string WorkstationName { get; }
        IItemContainer[] GetContainers();

        public bool InUse { get; }
    }
}
