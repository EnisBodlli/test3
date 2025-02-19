using UnityEngine;
using UnityEngine.UI;

namespace PolymindGames.UISystem
{
    public sealed class ButtonItemActionUI : ItemActionUI
    {
        [SpaceArea]

        [SerializeField]
        private Button m_Button;


        protected override void OnAttachment()
        {
            base.OnAttachment();
            m_Button.onClick.AddListener(StartAction);
        }

        protected override bool GetEnabledState()
        {
            bool enabled = m_ItemSlot != null
                && m_ItemSlot.HasItem
                && m_ItemAction != null
                && m_ItemAction.IsViableForItem(Player, m_ItemSlot);

            return enabled;
        }
    }
}
