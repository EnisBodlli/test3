#if XWIZARD_GAMES_STP_MP
using UnityEngine;
using Mirror;
using PolymindGames.WieldableSystem;

namespace XWizardGames.STP_MP
{
    public class CarriableNetworked : NetworkBehaviour
    {
        [SerializeField] private Carriable m_Carriable;

        [Command(requiresAuthority = false)]
        public void DropObjectCommand(int count, float dropHeight)
        {
            m_Carriable.DropObjectServer(count, dropHeight);
        }

    }
}
#endif
