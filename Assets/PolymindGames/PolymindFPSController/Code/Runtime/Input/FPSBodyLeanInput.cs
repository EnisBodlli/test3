using UnityEngine;
using UnityEngine.InputSystem;

namespace PolymindGames.InputSystem.Behaviours
{
    [AddComponentMenu("Input/Lean Input")]
    public sealed class FPSBodyLeanInput : CharacterInputBehaviour
    {
        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_LeanAction;

        private IBodyLeanHandler m_LeanHandler;


        #region Initialization
        protected override void OnBehaviourEnabled(ICharacter character)
        {
            character.GetModule(out m_LeanHandler);
        }

        protected override void OnInputEnabled()
        {
            m_LeanHandler.SetLeanState(BodyLeanState.Center);
            m_LeanAction.RegisterStarted(OnBodyLean);
        }

        protected override void OnInputDisabled()
        {
            m_LeanHandler.SetLeanState(BodyLeanState.Center);
            m_LeanAction.UnregisterStarted(OnBodyLean);
        }
        #endregion

        #region Input Handling
        private void OnBodyLean(InputAction.CallbackContext context)
        {
            var leanState = m_LeanHandler.LeanState;
            var targetLeanState = (BodyLeanState)Mathf.CeilToInt(context.ReadValue<float>());

            if (leanState == targetLeanState)
                m_LeanHandler.SetLeanState(BodyLeanState.Center);
            else
            {
                if (leanState != BodyLeanState.Center)
                    m_LeanHandler.SetLeanState(BodyLeanState.Center);
                else
                    m_LeanHandler.SetLeanState(targetLeanState);
            }
        }
        #endregion
    }
}
