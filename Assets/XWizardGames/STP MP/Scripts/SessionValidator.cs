#if XWIZARD_GAMES_STP_MP
#if XWIZARD_GAMES_LOBBY_SYSTEM
#if XWIZARD_GAMES_RELAY_SYSTEM

using UnityEngine;
using XWizardGames.UnityGamingServicesAddons.LobbyAddon;
using XWizardGames.UnityGamingServicesAddons;

namespace XWizardGames.STP_MP
{
    public class SessionValidator : MonoBehaviour
    {
        [SerializeField]
        private RelayNetworkManager _relayManager;

        [SerializeField]
        private STP_MP_NetworkManager _nonRelayManager;

        private bool _isStarted;


        public void StartRelayHostOrClient()
        {
            if(_isStarted)
                return;

            _relayManager.gameObject.SetActive(true);
            _relayManager.OnJoinRoomCodeUpdated += UpdateLobbyRelayJoinCode;

            if(UnityServicesManager.Instance.LobbyController.IsLocalPlayerHostOfCurrentLobby && UnityServicesManager.Instance.LobbyController.HasSession)
            {
                _relayManager.StartRelayHost(UnityServicesManager.Instance.LobbyController.CurrentLobby.MaxPlayers);
            }

            if(UnityServicesManager.Instance.LobbyController.IsLocalPlayerHostOfCurrentLobby == false && UnityServicesManager.Instance.LobbyController.HasSession)
            {
                _relayManager.relayJoinCode = UnityServicesManager.Instance.LobbyController.RelayCode;
                _relayManager.JoinRelayServer();
            }

            _isStarted = true;
        }

        public void StartSingleplayer()
        {
            if(_isStarted)
                return;

            _nonRelayManager.gameObject.SetActive(true);
            _nonRelayManager.StartHost();

            _isStarted = true;
        }

        public async void StartServer()
        {
            if(_isStarted)
                return;

            await UnityServicesManager.Instance.LobbyController.CreateLobby(GameModeType.FreeRoam, 4);

            _relayManager.gameObject.SetActive(true);
            _relayManager.OnJoinRoomCodeUpdated += UpdateLobbyRelayJoinCode;

            _relayManager.StartRelayServer(UnityServicesManager.Instance.LobbyController.CurrentLobby.MaxPlayers);

            _isStarted = true;
        }

        private void UpdateLobbyRelayJoinCode(string relayCode)
        {
            UnityServicesManager.Instance.LobbyController.UpdateRelayJoinCode(relayCode);
        }
    }
}
#endif
#endif
#endif