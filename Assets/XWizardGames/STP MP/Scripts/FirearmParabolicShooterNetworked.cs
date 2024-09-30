#if XWIZARD_GAMES_STP_MP
using UnityEngine;
using Mirror;
using PolymindGames.WieldableSystem;

namespace XWizardGames.STP_MP
{
    public class FirearmParabolicShooterNetworked : NetworkBehaviour
    {
        [SerializeField] private FirearmParabolicShooter m_ParabolicShooter;

        public void SpawnProjectile(Vector3 rayorigin, Vector3 direction, float speedMod, float spread)
        {
            CmdSpawnProjectile(rayorigin, direction, speedMod, spread);
        }

        [Command]
        private void CmdSpawnProjectile(Vector3 rayorigin, Vector3 direction, float speedMod, float spread)
        {
            m_ParabolicShooter.SpawnProjectileNetwork(rayorigin, direction, speedMod, spread);
        }
    }
}
#endif