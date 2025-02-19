using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    public interface IProjectileEffect
    {
        void DoHitEffect(RaycastHit hit, Vector3 hitDirection, float speed, float travelledDistance, ICharacter source);

        void Attach();
        void Detach();
    }
}