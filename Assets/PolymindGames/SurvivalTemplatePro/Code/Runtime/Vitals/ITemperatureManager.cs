using UnityEngine.Events;

namespace PolymindGames
{
    public interface ITemperatureManager : ICharacterModule
    {
        float Temperature { get; set; }

        event UnityAction<float> TemperatureChanged;
    }
}