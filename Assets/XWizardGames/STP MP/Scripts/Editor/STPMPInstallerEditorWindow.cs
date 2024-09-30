// Written by XWizardGames @Ronit
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.IO;


namespace XWizardGames.STP_MP
{
    [InitializeOnLoad]
    public class DisplaySTPMPInstallWizardOnLoad { 
        static DisplaySTPMPInstallWizardOnLoad()
        {
            if(PlayerPrefs.GetInt("DisplayedSTPMP_EDITOR_WINDOW") == 0)
            {
                PlayerPrefs.SetInt("DisplayedSTPMP_EDITOR_WINDOW", 1);
                STPMPInstallerEditorWindow.OpenEditorWindow();
            }
        }
    }


    public class STPMPInstallerEditorWindow : EditorWindow
    {
        private const string LOBBY_PACKAGE = "com.unity.services.lobby";
        private const string VIVOX_PACKAGE = "com.unity.services.vivox";
        private const string RELAY_PACKAGE = "com.unity.services.relay";
        private const string AUTHENTICATION_PACKAGE = "com.unity.services.authentication";
        private const string STP_MP_PACKAGE = "STP_MP";

        private const string LOBBY_SCRIPTING_SYMBOL = "XWIZARD_GAMES_LOBBY_SYSTEM";
        private const string VIVOX_SCRIPTING_SYMBOL = "XWIZARD_GAMES_VIVOX_SYSTEM";
        private const string RELAY_SCRIPTING_SYMBOL = "XWIZARD_GAMES_RELAY_SYSTEM";
        private const string AUTHENTICATION_SCRIPTING_SYMBOL = "XWIZARD_GAMES_AUTHENTICATION_SYSTEM";
        private const string STP_MP_SCRIPTING_SYMBOL = "XWIZARD_GAMES_STP_MP";

        private const string STP_MP_PACKAGE_LOCATION = "Assets/XWizardGames/STP MP/_Main/STP MP 1.3 Store Addon.unitypackage";
        private const string RELAY_ADDON_PACKAGE_LOCATION = "Assets/XWizardGames/STP MP/_Main/Relay Addon.unitypackage";


        private const string DISCORD_URL = "https://discord.com/invite/pJVTStdetN";
        private const string WEBSITE_URL = "https://xwizardgames.com/";


        public Label m_PackageName;
        public Label m_PackageDescription;
        public Button authButton;
        public Button lobbyButton;
        public Button relayButton;
        public Button vivoxButton;
        public Button stpMpBaseInstallerButton;
        public Button installButton;
        public Button discordButton;
        public Button websiteButton;

        [MenuItem("Tools/STP MP Installer")]
        public static void OpenEditorWindow()
        {
            STPMPInstallerEditorWindow wnd = GetWindow<STPMPInstallerEditorWindow>();
            wnd.titleContent = new GUIContent("STP MP Installer");

            wnd.maxSize = new Vector2(800, 600);
            wnd.minSize = wnd.maxSize;
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/XWizardGames/STP MP/Scripts/Editor/Resources/STP MP Services Editor.uxml");

            VisualElement tree = visualTree.Instantiate();
            root.Add(tree);

            m_PackageName = root.Q<Label>("package_title");
            m_PackageDescription = root.Q<Label>("description");
            stpMpBaseInstallerButton = root.Q<Button>("stp_mp_base");
            authButton = root.Q<Button>("authentication");
            lobbyButton = root.Q<Button>("lobby");
            relayButton = root.Q<Button>("relay");
            vivoxButton = root.Q<Button>("vivox");
            installButton = root.Q<Button>("install_button");

            discordButton = root.Q<Button>("discord");
            websiteButton = root.Q<Button>("website");

            authButton.clicked += OnAuthButtonClicked;
            lobbyButton.clicked += OnLobbyButtonClicked;
            relayButton.clicked += OnRelayButtonClicked;
            vivoxButton.clicked += OnVivoxButtonClicked;
            stpMpBaseInstallerButton.clicked += OnSTPMPButtonClicked;
            discordButton.clicked += OnDiscordButtonClicked;
            websiteButton.clicked += OnWebsiteButtonClicked;

            installButton.clicked += InstallCurrentPackage;

            OnSTPMPButtonClicked();
        }

        private string m_CurrentPackage = "com.unity.services.authentication";
        private string m_CurrentPackageScriptingSymbol = "XWIZARD_GAMES_LOBBY_SYSTEM";
        static AddRequest Request;



        private void UpdateInstallButtonText()
        {
            if (m_CurrentPackage.Equals(STP_MP_PACKAGE))
            {
                if (IsScriptingDefineSymbolDefined(STP_MP_SCRIPTING_SYMBOL))
                {
                    installButton.text = "Installed";
                    return;
                }
            }


            if (IsPackageInstalled(m_CurrentPackage))
            {
                installButton.text = "Installed";
            }
            else
            {
                installButton.text = "Install";

            }
        }
        public bool IsPackageInstalled(string packageId)
        {
            if (!File.Exists("Packages/manifest.json"))
                return false;

            string jsonText = File.ReadAllText("Packages/manifest.json");
            bool packageFound = jsonText.Contains(packageId);

            if(packageFound && HasSymbol(m_CurrentPackageScriptingSymbol))
            {
                return true;
            }

            return false;
        }
        public void OnSTPMPButtonClicked()
        {
            m_CurrentPackage = STP_MP_PACKAGE;
            m_PackageName.text = "STP MP Installer.";
            m_PackageDescription.text = "To set up STP MP in your project, press \"Install\" and import. This setup is mandatory before you can use STP MP. Other integrations, such as relay, vivox, and lobby, are not mandatory. However if you want to use Main Menu , you will need to install those. ";
            UpdateInstallButtonText();
        }
        public void OnAuthButtonClicked()
        {
            m_CurrentPackage = AUTHENTICATION_PACKAGE;

            m_CurrentPackageScriptingSymbol = AUTHENTICATION_SCRIPTING_SYMBOL;

            m_PackageName.text = "Authencation Package";
            m_PackageDescription.text = "Enable authencation for allowing users to sign-in into the game. This is a must install package if you are using any other of the services (lobby, relay, vivox)";

            UpdateInstallButtonText();
        }
        public void OnLobbyButtonClicked()
        {
            m_CurrentPackage = LOBBY_PACKAGE;

            m_CurrentPackageScriptingSymbol = LOBBY_SCRIPTING_SYMBOL;

            m_PackageName.text = "Lobby Package";
            m_PackageDescription.text = "Enable players to find, create, and join lobbies with Lobby. Add Lobby to your multiplayer game to empower players to create the gaming experiences they want. Features include Quick Join, Public Lobby, and Private Match to allow flexibility in how players want to game with each other.";

            UpdateInstallButtonText();

        }
        public void OnRelayButtonClicked()
        {
            m_CurrentPackage = RELAY_PACKAGE;
            m_CurrentPackageScriptingSymbol = RELAY_SCRIPTING_SYMBOL;

            m_PackageName.text = "Relay Package";
            m_PackageDescription.text = "Connect the players of your peer-hosted games with Relay. Your games will also benefit from improved reliability, privacy and the ability to communicate across platforms. To get started, enable listen-server UDP networking for your project on the Unity Dashboard.\r\n\r\nFeatures include an allocation service to join game hosts via code, multi-region support, and more. \r\n\r\nThis integration is based on unity mirror relay sample. https://docs.unity.com/ugs/manual/relay/manual/mirror";

            UpdateInstallButtonText();

        }
        public void OnVivoxButtonClicked()
        {
            m_CurrentPackage = VIVOX_PACKAGE;
            m_CurrentPackageScriptingSymbol = VIVOX_SCRIPTING_SYMBOL;

            m_PackageName.text = "Vivox Package";
            m_PackageDescription.text = "Unity's voice and text chat service for multiplayer communication offers voice chat and direct message text service with a managed hosted solution. Plug into your game and configure your project settings to add communications to your project immediately. Connect an unlimited number of users in 2D and 3D channels. Monitor concurrency on your dashboard. Add a custom UI to allow players to control voice volume, mute, and add audio effects to voice channels and manage them. Place users in team chat and allow them to participate in multiple channels";

            UpdateInstallButtonText();

        }
        public void OnDiscordButtonClicked()
        {
            Application.OpenURL(DISCORD_URL);
        }
        public void OnWebsiteButtonClicked()
        {
            Application.OpenURL(WEBSITE_URL);
        }
        private void InstallCurrentPackage()
        {
            if (m_CurrentPackage == STP_MP_PACKAGE)
            {
                InstallSTPMP();
                return;
            }
            else
            {
                if (IsPackageInstalled(m_CurrentPackage))
                {
                    Debug.Log("Package is already installed");
                    return;
                }

                installButton.text = "Installing...";

                AddSymbol(m_CurrentPackageScriptingSymbol);

                Request = Client.Add(m_CurrentPackage);

                

                EditorApplication.update += Progress;

                if (m_CurrentPackage == RELAY_PACKAGE)
                {
                    AssetDatabase.ImportPackage(RELAY_ADDON_PACKAGE_LOCATION, true);

                }
            }
        }

        private void AddSymbol(string symbol)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!definesString.Contains(symbol))
            {
                if (definesString.Length > 0)
                {
                    definesString += ";";
                }
                definesString += symbol;

                // Set the modified symbols back
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, definesString);
            }
        }

        private bool HasSymbol(string symbol)
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return definesString.Contains(symbol);
        }
        private bool IsScriptingDefineSymbolDefined(string symbol)
        {
            // Get the scripting define symbols for the current build target group
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Split the define symbols string into an array of symbols
            string[] symbols = defineSymbols.Split(';');

            // Check if the symbol to check is present in the array of symbols
            foreach (string s in symbols)
            {
                if (s.Trim() == symbol)
                {
                    return true;
                }
            }

            return false;
        }

        private void InstallSTPMP()
        {
            Debug.Log("Instlaling STP MP. Hold on...");
            // Get the current symbols for the active build target
            AddSymbol(STP_MP_SCRIPTING_SYMBOL);


            AssetDatabase.ImportPackage(STP_MP_PACKAGE_LOCATION, true);
        }
        static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                {
                    Debug.Log("Installed: " + Request.Result.packageId);

                    if (Request.Result.packageId.Contains(RELAY_PACKAGE))
                    {
                        Request = Client.Add("com.unity.jobs");
                    }

                }
                else if (Request.Status >= StatusCode.Failure)
                {
                    Debug.Log(Request.Error.message);
                }
                EditorApplication.update -= Progress;
            }
        }
    }
}