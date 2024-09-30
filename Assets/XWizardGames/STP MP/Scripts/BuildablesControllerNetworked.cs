#if XWIZARD_GAMES_STP_MP
using UnityEngine;
using Mirror;
using PolymindGames.BuildingSystem;

namespace XWizardGames.STP_MP
{
    public class BuildablesControllerNetworked : NetworkBehaviour
    {

        [Command(requiresAuthority = false)]
        public void SpawnBuildableCmd(int buildableId, Vector3 position, Quaternion rotation)
        {
            BuildableDefinition def = BuildableDefinition.GetWithId(buildableId);

            if (def == null) return;

            Buildable go = Instantiate(def.Prefab, position, rotation);
            NetworkServer.Spawn(go.gameObject);
        }

    }
}
#endif