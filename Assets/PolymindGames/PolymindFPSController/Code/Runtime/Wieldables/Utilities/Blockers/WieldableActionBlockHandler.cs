using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames.WieldableSystem
{
    public sealed class ActionBlockHandler
    {
        public bool IsBlocked { get; private set; }


        private readonly List<Object> m_Blockers = new();
        private UnityAction m_BlockAction;


        public void SetBlockCallback(UnityAction callback) => m_BlockAction = callback;

        public void AddBlocker(Object blocker)
        {
            if (m_Blockers.Contains(blocker))
                return;

            m_Blockers.Add(blocker);

            if (IsBlocked == false)
                m_BlockAction?.Invoke();

            IsBlocked = true;
        }

        public void RemoveBlocker(Object blocker)
        {
            if (!m_Blockers.Contains(blocker))
                return;

            m_Blockers.Remove(blocker);
            IsBlocked = m_Blockers.Count > 0;
        }
    }
}