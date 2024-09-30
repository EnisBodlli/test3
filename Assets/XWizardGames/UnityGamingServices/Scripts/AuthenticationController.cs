#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Authentication;
using Unity.Services.Core;
#endif

using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using PolymindGames;

// Some parts of the code is taken from Unity Auth Docs
namespace XWizardGames.UnityGamingServicesAddons.AuthenticationAddon
{
    public class AuthenticationController : ServiceController
    {
        public override void Initialize()
        {
            // Component is ready.
        }
#if XWIZARD_GAMES_LOBBY_SYSTEM

        [SerializeField] private string m_ProfileName = "XWizard";
        [SerializeField] private bool m_UseNewProfile = false;

        public UnityEvent OnSignIn;
        public UnityEvent OnSignUp;
        public UnityEvent OnNameFetched;


        private void Awake()
        {
            UnityServices.InitializeAsync();
        }

        private void OnEnable()
        {
            OnSignIn.AddListener(OnSignInCallback);
        }

        private void OnDisable()
        {
            OnSignIn.RemoveListener(OnSignInCallback);
        }

        private string playerName;
        private string playerId;

        public string PlayerName => AuthenticationService.Instance.PlayerName;
        public string PlayerID => AuthenticationService.Instance.PlayerId;


        private void OnSignInCallback()
        {

        }

        public async void SignInAnonymously()
        {
            AuthenticationService.Instance.SwitchProfile(m_ProfileName + Mathf.RoundToInt(UnityEngine.Random.value * 1000).ToString());

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            OnSignIn?.Invoke();
        }

        public async Task SignUpWithUsernamePassword(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                await AuthenticationService.Instance.GetPlayerNameAsync();

                Debug.Log("SignUp is successful.");
                OnSignUp?.Invoke();
                OnSignIn?.Invoke();

            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
        public async Task LoginInWithUsernamePasswordAsync(string username, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                await AuthenticationService.Instance.GetPlayerNameAsync();
                Debug.Log("SignInAnonymously is successful." + AuthenticationService.Instance.PlayerName);

                OnSignIn?.Invoke();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        public async Task<string> UpdateDisplayName(string playerNameNew)
        {
            Debug.Log("Updating name");

            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerNameNew);
            string name = await AuthenticationService.Instance.GetPlayerNameAsync();
            Debug.Log("Updated name is " + name);
            return name;
        }
#endif
    }


}