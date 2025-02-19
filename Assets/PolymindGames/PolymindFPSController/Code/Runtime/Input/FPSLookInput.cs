﻿using UnityEngine;
using UnityEngine.InputSystem;

namespace PolymindGames.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Look Input")]
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/camera#player-look-input-behaviour")]
    public class FPSLookInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_LookInput;

        private ILookHandler m_LookHandler;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_LookHandler);
        }

        protected override void OnInputEnabled()
        {
            m_LookInput.Enable();
            m_LookHandler.SetLookInput(GetInput);
        }

        protected override void OnInputDisabled()
        {
            m_LookInput.TryDisable();
            m_LookHandler.SetLookInput(null);
        }
        #endregion

        #region Input Handling
        private Vector2 GetInput()
        {
            Vector2 lookInput = m_LookInput.action.ReadValue<Vector2>() * 0.1f;
            lookInput.ReverseVector();

            return lookInput;
        }
        #endregion
    }
}