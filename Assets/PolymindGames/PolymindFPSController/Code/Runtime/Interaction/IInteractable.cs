using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames
{
    public interface IInteractable
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        bool InteractionEnabled { get; set; }
        float HoldDuration { get; }

        event UnityAction<ICharacter> Interacted;
        event UnityAction InteractionEnabledChanged;


        /// <summary>
        /// Called when a character interacts with this object.
        /// </summary>
        void OnInteract(Character character);
        //void InteractCommand(Character character);
    }
}