using UnityEngine;
using Mirror;


namespace PolymindGames.ResourceGathering
{
    public abstract class GatherableBehaviour : NetworkBehaviour
    {
        protected IGatherable Gatherable { get; private set; }


        public void InitializeBehaviour(IGatherable gatherable)
        {
            if (Gatherable != null)
            {
                Debug.LogError($"The behaviour of type ''{this.GetType().Name}'' has already been initialized on: ''{gameObject.name}''.");
                return;
            }

            this.Gatherable = gatherable;
        }

        public abstract void DoHitEffects(DamageContext damageInfo);
        public abstract void DoDestroyEffects(DamageContext damageInfo);
    }
}