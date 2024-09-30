#if XWIZARD_GAMES_STP_MP
using Mirror;
using UnityEngine;

namespace PolymindGames
{
    /// <summary>
    /// Basic Singleton
    /// </summary>
    public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T> 
	{
		public static bool HasInstance => s_Instance != null;
		public static T Instance
		{
			get
			{
				if (s_Instance == null)
					SetInstance(FindObjectOfType<T>());

				return s_Instance;
			}
		}
		
		private static T s_Instance;
		private bool m_IsInitialized;


		private void Awake() => SetInstance((T)this);

		private static void SetInstance(T instance)
		{
			if (instance == null)
				return;

			if (s_Instance != null && s_Instance != instance)
			{
				Destroy(instance.gameObject);
				return;
			}

			s_Instance = instance;

			if (!s_Instance.m_IsInitialized)
			{
				s_Instance.OnAwake();
				s_Instance.m_IsInitialized = true;
			}
		}

		protected virtual void OnAwake() { }
	}
}

#endif