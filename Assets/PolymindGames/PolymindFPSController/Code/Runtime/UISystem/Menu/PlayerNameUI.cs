using TMPro;
using UnityEngine;

using XWizardGames.UnityGamingServicesAddons;

namespace PolymindGames.UISystem
{
    public class PlayerNameUI : MonoBehaviour
    {
        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private TextMeshProUGUI m_NameText;

        [SerializeField]
        private TMP_InputField m_NameInputField;

        // private const string k_PlayerNamePrefName = "POLYMIND_PLAYER_NAME";
        private const string k_UnnamedPlayerName = "Unnamed";


        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void SavePlayerNameFromField()
        {
            if (string.IsNullOrEmpty(m_NameInputField.text))
                return;

            SavePlayerName(m_NameInputField.text);
        }

        public async void SavePlayerName(string name)
        {
#if XWIZARD_GAMES_LOBBY_SYSTEM
            string newName = await UnityServicesManager.Instance.AuthenticationController.UpdateDisplayName(name);
            //PlayerPrefs.SetString(k_PlayerNamePrefName, name);

            m_NameText.text = newName;
#endif
        }

        public void ResetPlayerNameField()
        {
#if XWIZARD_GAMES_LOBBY_SYSTEM

            //m_NameInputField.text = PlayerPrefs.GetString(k_PlayerNamePrefName);
            m_NameInputField.text = UnityServicesManager.Instance.AuthenticationController.PlayerName;
#endif
        }

        private void Start()
        {
#if XWIZARD_GAMES_LOBBY_SYSTEM

            UnityServicesManager.Instance.AuthenticationController.OnSignIn.AddListener(GetPlayerName);
#endif
            /*            UnityServicesManager.ins
                        if (!PlayerPrefs.HasKey(k_PlayerNamePrefName) || PlayerPrefs.GetString(k_PlayerNamePrefName) == string.Empty)
                        {
                            SavePlayerName(k_UnnamedPlayerName);
                            m_Panel.Show(true);
                        }
                        else
                            m_NameInputField.text = m_NameText.text = PlayerPrefs.GetString(k_PlayerNamePrefName);*/
        }
        private void GetPlayerName()
        {
#if XWIZARD_GAMES_LOBBY_SYSTEM

            UnityServicesManager.Instance.AuthenticationController.OnSignIn.RemoveListener(GetPlayerName);

            string playerName = UnityServicesManager.Instance.AuthenticationController.PlayerName;

            Debug.Log("Player name is " + playerName);
            if (string.IsNullOrEmpty(playerName))
            {
                m_Panel.Show(true);
            }
            else
            {
                m_NameInputField.text = playerName;
                m_NameText.text = playerName;
            }
#endif
        }
    }
}
