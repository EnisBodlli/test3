using UnityEngine;
using XWizardGames.UnityGamingServicesAddons.AuthenticationAddon;
using XWizardGames.UnityGamingServicesAddons.LobbyAddon;
using XWizardGames.UnityGamingServicesAddons.VivoxAddon;
using System;

using Unity.Services.Authentication;


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
                if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType<UnityServicesManager>(true);
                    if (m_Instance == null)
                    {
                        // Create the instance if it's not found
                        GameObject obj = new GameObject("UnityServicesManager");
                        m_Instance = obj.AddComponent<UnityServicesManager>();
                        DontDestroyOnLoad(obj);
                        Debug.LogError(m_Instance + "enesi");

                        // Call InitializeUnityService immediately after creating the instance
                        m_Instance.InitalizeUnityService();
                    }
                    else
                    {
                        // Call InitializeUnityService if the instance was found but needs to be initialized
                        m_Instance.InitalizeUnityService();
                    }

                    m_Instance.gameObject.SetActive(true);
                }
                return m_Instance;
            }
        }

        public async void InitalizeUnityService()
        {
            try
            {
                // Initialize Unity Services
                await UnityServices.InitializeAsync();
                Debug.Log("Unity Services Initialized");

                // Ensure user is authenticated
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
            }

            m_AuthenticationController.Initialize();
            m_LobbyController.Initialize();

        }
        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            m_Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.LogError("UnityServicesManager initialized and set to DontDestroyOnLoad.");
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
            Debug.LogError("Service ready to be used");
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