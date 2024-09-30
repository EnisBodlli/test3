using UnityEngine.Events;

namespace PolymindGames
{
    public interface IStaminaController : ICharacterModule
    {
        /// <summary>
        /// Current stamina, 0 - 1 range
        /// </summary>
        float Stamina { get; set; }

        event UnityAction<float> StaminaChanged;
    }
}