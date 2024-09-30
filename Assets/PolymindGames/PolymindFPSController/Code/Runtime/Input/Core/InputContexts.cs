using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolymindGames.InputSystem
{
    [CreateAssetMenu(menuName = "Polymind Games/Input/Input Contexts Asset", fileName = "(InputContexts) ")]
    public class InputContexts : ScriptableObject
    {
        [SerializeField]
        private InputContextGroup m_Groups;

        [SerializeField]
        private InputContext[] m_Contexts;
    }
}
