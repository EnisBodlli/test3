﻿using PolymindGames.InventorySystem;
using UnityEngine.Events;

namespace PolymindGames
{
    /// <summary>
    /// TODO: Implement unlocked recipes and crafting queue.
    /// </summary>
    public interface ICraftingManager : ICharacterModule
    {
        bool IsCrafting { get; }

        event UnityAction<ItemDefinition> CraftingStart;
        event UnityAction CraftingEnd;

        void Craft(ItemDefinition itemInfo);
        void CancelCrafting();
    }
}