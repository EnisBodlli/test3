using UnityEngine.AI;
using UnityEngine;
using Mirror;

namespace PolymindGames.BuildingSystem
{
    public class FreeBuildable : Buildable
    {
		public override void OnCreated(bool playEffects = true)
		{
			EnableColliders(false);

			gameObject.SetLayerRecursively(BuildingManager.BuildablePreviewLayer);
			MaterialEffect.EnableCustomEffect(BuildingManager.PlacementAllowedMaterialEffect);

			GetComponent<NavMeshObstacle>().enabled = false;
		}

		public override void OnPlaced(bool playEffects = true)
		{
			EnableColliders(true);

			if (playEffects)
				DoPlacementEffects();

			GetComponent<NavMeshObstacle>().enabled = false;
		}

		public override void OnBuilt(bool playEffects = true)
		{
			gameObject.SetLayerRecursively(BuildingManager.BuildableLayer);
			MaterialEffect.DisableActiveEffect();

			if (playEffects)
				DoBuildEffects();

			GetComponent<NavMeshObstacle>().enabled = true;

			Player.LocalPlayer.BuildablesControllerNetworked.SpawnBuildableCmd(Definition.Id, transform.position, transform.rotation);

			Debug.Log("Cmd request was sent");
			Destroy(gameObject);
        }


	}
}