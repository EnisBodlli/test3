using PolymindGames.UISystem;
using System.Collections.Generic;
using TMPro;

#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Lobbies.Models;
#endif
#if XWIZARD_GAMES_VIVOX_SYSTEM
#endif

using UnityEngine;
using UnityEngine.UI;

namespace XWizardGames.UnityGamingServicesAddons.LobbyAddon
{
    public class UIRoom : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_RoomNameText;

        [SerializeField]
        private UIRoomPlayerElement m_UIRoomPlayerElementPrefab;

        [SerializeField] 
        private Transform m_PlayersScrollContent;

        [SerializeField]
        private TMP_Dropdown m_GameModeDropDown;

        [SerializeField]
        private TMP_Dropdown m_MapDropDown;

        [SerializeField]
        private TMP_Dropdown m_MicDropDown;

        [SerializeField]
        private TMP_Text m_PlayersCount;

        [SerializeField]
        private Button m_StartButton;

        [SerializeField]
        private AnimatedPanelUI m_SessionPanel;


#if XWIZARD_GAMES_LOBBY_SYSTEM

        private void Start()
        {
            UnityServicesManager.Instance.LobbyController.OnJoinedLobby += OnJoinedLobby;
            UnityServicesManager.Instance.LobbyController.OnCurrentLobbyInfoUpdated += OnLobbyUpdated;

            UpdateMapDropdown();
            //UpdateMicDropdown();

            m_StartButton.onClick.AddListener(() => UnityServicesManager.Instance.LobbyController.StartGame());
        }

        private void OnDestroy()
        {
            UnityServicesManager.Instance.LobbyController.OnJoinedLobby -= OnJoinedLobby;
            UnityServicesManager.Instance.LobbyController.OnCurrentLobbyInfoUpdated -= OnLobbyUpdated;
        }

        List<string> micdevicesOptions = new List<string>();

        private void UpdateMicDropdown()
        {
#if XWIZARD_GAMES_VIVOX_SYSTEM

            m_MicDropDown.ClearOptions();
            Debug.Log(VivoxService.Instance);
            foreach(VivoxInputDevice device in VivoxService.Instance.AvailableInputDevices){
                micdevicesOptions.Add(device.DeviceName);
            }
            m_MicDropDown.AddOptions(micdevicesOptions);
            m_MicDropDown.onValueChanged.AddListener(OnMicInputDeviceChanged);

#endif
        }

        private void OnMicInputDeviceChanged(int index)
        {
#if XWIZARD_GAMES_VIVOX_SYSTEM

            VivoxService.Instance.SetActiveInputDeviceAsync(VivoxService.Instance.AvailableInputDevices.First(device => device.DeviceName == micdevicesOptions[index]));
#endif
        }

        private void UpdateMapDropdown()
        {
            m_MapDropDown.ClearOptions();
            m_MapDropDown.AddOptions(UnityServicesManager.Instance.LobbyController.SceneNames);
            m_MapDropDown.onValueChanged.AddListener(OnMapChanged);
        }

        private void OnMapChanged(int value)
        {
            UnityServicesManager.Instance.LobbyController.ChangeMap(value);
        }

        public void OnJoinedLobby(Lobby joinedlobby)
        {
            GetComponent<AnimatedPanelUI>().Show(true);

            RefreshPlayersUI(joinedlobby);

            UpdateRoomUI(joinedlobby);
            UpdateMicDropdown();
        }

        private void RefreshPlayersUI(Lobby joinedlobby)
        {
            foreach (Transform t in m_PlayersScrollContent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player p in joinedlobby.Players)
            {
                UIRoomPlayerElement element = Instantiate(m_UIRoomPlayerElementPrefab, m_PlayersScrollContent);
                element.Initialize(p);
            }
        }

        private void UpdateRoomUI(Lobby lobby)
        {
            //m_GameModeDropDown.SetValueWithoutNotify();
            //m_GameModeDropDown.SetValueWithoutNotify();
            int dropdownindex = 0;

            foreach(string t in UnityServicesManager.Instance.LobbyController.SceneNames)
            {
                if (t.Equals(lobby.Data[LobbyController.MAP_KEY].Value))
                    break;

                dropdownindex++;
            }

            RefreshPlayersUI(lobby);

            m_MapDropDown.SetValueWithoutNotify(dropdownindex);
            m_PlayersCount.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }

        private void OnLobbyUpdated(Lobby joinedlobby)
        {
            UpdateRoomUI(joinedlobby);
        }

        public async void Leave()
        {
            bool left = await UnityServicesManager.Instance.LobbyController.LeaveLobby();

            if (left == false)
                return;

            GetComponent<AnimatedPanelUI>().Show(false);

            m_SessionPanel.Show(true);
        }
#endif
    }
}