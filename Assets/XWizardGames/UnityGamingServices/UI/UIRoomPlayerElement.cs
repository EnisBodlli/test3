using System.Collections;
using System.Collections.Generic;
using TMPro;
#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Lobbies.Models;
#endif
using UnityEngine;

namespace XWizardGames.UnityGamingServicesAddons.LobbyAddon
{
    public class UIRoomPlayerElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_PlayerNameText;
        [SerializeField] private TMP_Text m_ExperienceText;


#if XWIZARD_GAMES_LOBBY_SYSTEM

        public void Initialize(Player lobbyPlayer)
        {
            m_PlayerNameText.text = lobbyPlayer.Data[LobbyController.PLAYER_NAME_KEY].Value;
            m_ExperienceText.text = "Master";
        }
#endif
    }
}
