using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    [DisallowMultipleComponent]
    public abstract class WieldableAnimator : WieldableBehaviour
    {
        public abstract Animator Animator { get; }
    }
}