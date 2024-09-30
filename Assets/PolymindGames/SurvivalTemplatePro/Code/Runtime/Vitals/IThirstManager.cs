using UnityEngine.Events;

namespace PolymindGames
{
    public interface IThirstManager : ICharacterModule
    {
        float Thirst { get; set; }
        float MaxThirst { get; set;  }
    }
}