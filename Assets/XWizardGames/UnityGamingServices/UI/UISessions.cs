using PolymindGames.UISystem;
using System.Collections;
using System.Collections.Generic;
#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Lobbies.Models;
#endif
using UnityEngine;

namespace XWizardGames.UnityGamingServicesAddons.LobbyAddon {
    public class UISessions : MonoBehaviour
    {
        [SerializeField] private UISessionElement m_UISessionElementPrefab;
        [SerializeField] private Transform m_scrollContent;

#if XWIZARD_GAMES_LOBBY_SYSTEM

        private void OnEnable()
        {
        }
        private void Start()
        {
            UnityServicesManager.Instance.LobbyController.OnLobbyListUpdated += OnLobbyListUpdated;
        }
        public void RefreshLobby()
        {
            UnityServicesManager.Instance.LobbyController.RefreshLobbyList(GameModeType.FreeRoam);

        }

        private void OnDisable()
        {
            UnityServicesManager.Instance.LobbyController.OnLobbyListUpdated -= OnLobbyListUpdated;

        }

        public void OnLobbyListUpdated(QueryResponse response)
        {
            foreach (Transform t in m_scrollContent)
            {
                Destroy(t.gameObject);
            }

            foreach (var item in response.Results)
            {
                UISessionElement element = Instantiate(m_UISessionElementPrefab, m_scrollContent);
                element.Initialize(item, this);
            }

        }

        public void CloseWindow()
        {
            GetComponent<AnimatedPanelUI>().Show(false);
        }
#endif

    }
}
