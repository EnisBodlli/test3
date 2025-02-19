using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames.WieldableSystem
{
    public sealed class WieldableObjectVFX : WieldableVFXBehaviour<Transform>
	{
		[SerializeField]
		private UnityEvent m_OnSpawn;


		protected override Transform SpawnEffect()
		{
			m_OnSpawn.Invoke();
			return base.SpawnEffect();
		}
	}
}
