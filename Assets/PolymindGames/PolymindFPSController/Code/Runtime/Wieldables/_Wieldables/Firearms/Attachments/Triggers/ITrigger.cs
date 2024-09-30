using UnityEngine.Events;

namespace PolymindGames.WieldableSystem
{
    public interface ITrigger
    {
        bool IsTriggerHeld { get; }

        event UnityAction<float> Shoot;


        void HoldTrigger();
        void ReleaseTrigger();

        void Attach();
        void Detach();
    }
}