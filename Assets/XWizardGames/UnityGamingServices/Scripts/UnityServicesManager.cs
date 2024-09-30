using UnityEngine;
using XWizardGames.UnityGamingServicesAddons.AuthenticationAddon;
using XWizardGames.UnityGamingServicesAddons.LobbyAddon;
using XWizardGames.UnityGamingServicesAddons.VivoxAddon;

#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Core;
#endif


namespace XWizardGames.UnityGamingServicesAddons
{
    public class UnityServicesManager : MonoBehaviour
    {
        [SerializeField] private AuthenticationController m_AuthenticationController;

        [SerializeField] private LobbyController m_LobbyController;
        public AuthenticationController AuthenticationController => m_AuthenticationController;

        public LobbyController LobbyController => m_LobbyController;

        [SerializeField] private VivoxController m_VivoxController;
        public VivoxController VivoxController => m_VivoxController;

        private static UnityServicesManager m_Instance;
        public static UnityServicesManager Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(gameObject);
            }

            m_Instance = this;
            DontDestroyOnLoad(this);
        }
        private async void Start()
        {
#if XWIZARD_GAMES_LOBBY_SYSTEM

            await UnityServices.InitializeAsync();

            m_AuthenticationController.Initialize();
            m_LobbyController.Initialize();
#endif

#if XWIZARD_GAMES_VIVOX_SYSTEM
            m_VivoxController.Initialize();
#endif
            Debug.Log("Service ready to be used");
        }

        private void OnValidate()
        {
#if XWIZARD_GAMES_LOBBY_SYSTEM
            m_LobbyController = GetComponent<LobbyController>();
#endif

#if XWIZARD_GAMES_VIVOX_SYSTEM
            m_VivoxController = GetComponent<VivoxController>();
#endif
        }
    }
}