using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    public abstract class CarriableActionBehaviour : MonoBehaviour
    {
        protected Carriable Carriable { get; private set; }


        /// <summary>
        /// Does an action using this carriable.
        /// </summary>
        public abstract bool TryUseCarriable();

        protected virtual void Awake()
        {
            Carriable = GetComponent<Carriable>();
        }
    }
}
