using UnityEngine;
using UnityEngine.InputSystem;

namespace PolymindGames.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Interaction Input")]
    [HelpURL(
        "https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/interaction#player-interaction-input-behaviour")]
    public class FPSInteractionInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_InteractInput;

        private IInteractionHandler m_InteractionHandler;
        
        
        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_InteractionHandler);
        }

        protected override void OnInputEnabled()
        {
            m_InteractInput.RegisterStarted(OnInteractStart);
            m_InteractInput.RegisterCanceled(OnInteractStop);
            m_InteractionHandler.InteractionEnabled = true;
        }

        protected override void OnInputDisabled()
        {
            m_InteractInput.UnregisterStarted(OnInteractStart);
            m_InteractInput.UnregisterCanceled(OnInteractStop);
            m_InteractionHandler.InteractionEnabled = false;
        }
        #endregion

        #region Input Handling
        private void OnInteractStart(InputAction.CallbackContext obj) => m_InteractionHandler.StartInteraction();
        private void OnInteractStop(InputAction.CallbackContext obj) => m_InteractionHandler.StopInteraction();
        #endregion
    }
}
