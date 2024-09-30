using UnityEngine;

namespace PolymindGames
{
    public interface ICharacterModule
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}