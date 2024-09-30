using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Lobbies.Models;
#endif

namespace XWizardGames.UnityGamingServicesAddons.LobbyAddon {
    public class UISessionElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_RoomNameText;
        [SerializeField] private TMP_Text m_MapNameText;
        [SerializeField] private TMP_Text m_GameModeText;
        [SerializeField] private TMP_Text m_SlotsText;
        [SerializeField] private TMP_Text m_LanguageText;
        [SerializeField] private Button m_JoinButton;

#if XWIZARD_GAMES_LOBBY_SYSTEM

        public void Initialize(Lobby lobbyInfo, UISessions session)
        {
            m_JoinButton.onClick.AddListener(() => UnityServicesManager.Instance.LobbyController.JoinLobbyWithID(lobbyInfo.Id));
            m_JoinButton.onClick.AddListener(() => session.CloseWindow());
            m_RoomNameText.text = lobbyInfo.Name;
            m_MapNameText.text = lobbyInfo.Data[LobbyController.MAP_KEY].Value.ToString();
            m_GameModeText.text = lobbyInfo.Data[LobbyController.GAME_MODE_KEY].Value.ToString();
            m_SlotsText.text = lobbyInfo.Players.Count.ToString() + "/" + lobbyInfo.MaxPlayers;
            m_LanguageText.text = lobbyInfo.Data[LobbyController.LOBBY_LANGUAGE].Value.ToString();
        }
#endif
    }
}
