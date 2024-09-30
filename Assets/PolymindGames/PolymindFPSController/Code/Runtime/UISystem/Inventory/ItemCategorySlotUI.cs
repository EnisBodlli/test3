using PolymindGames.InventorySystem;
using UnityEngine;

namespace PolymindGames.UISystem
{
    [RequireComponent(typeof(SelectableUI))]
    public class ItemCategorySlotUI : SlotUI<ItemCategoryDefinition>
    {
        public ItemCategoryDefinition ItemCategory => Data;


        public void SetCategory(ItemCategoryDefinition category)
        {
            SetData(category);
        }
    }
}
