using UnityEngine;
using System;

namespace PolymindGames.InputSystem
{
    [CreateAssetMenu(menuName = "Polymind Games/Input/Input Context", fileName = "(InputContext) ")]
    public sealed class InputContext : InputContextBase
    {
        internal override InputContextBase[] GetSubContexts() => Array.Empty<InputContextBase>();
    }
}
