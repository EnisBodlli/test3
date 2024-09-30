using PolymindGames.WieldableSystem;
using UnityEngine.Events;

namespace PolymindGames
{
    public interface IWieldableHealingHandler : ICharacterModule
    {
        event UnityAction HealsCountChanged;
        event UnityAction<HealingItem> OnHeal;


        bool TryHeal();
        int GetHealsCount();
    }
}
