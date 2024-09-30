using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if XWIZARD_GAMES_VIVOX_SYSTEM
using Unity.Services.Vivox;
#endif

using System.Threading.Tasks;
using System.Linq;

namespace XWizardGames.UnityGamingServicesAddons.VivoxAddon
{
    public class VivoxController : ServiceController
    {
        public override void Initialize()
        {
#if XWIZARD_GAMES_VIVOX_SYSTEM
            UnityServicesManager.Instance.AuthenticationController.OnSignIn.AddListener(OnUserLogin);
#endif

        }
#if XWIZARD_GAMES_VIVOX_SYSTEM

        private async void OnUserLogin()
        {
            await VivoxService.Instance.InitializeAsync();
           // SetVivoxInputMicrophone();


            VivoxService.Instance.LoggedIn += OnUserLoggedIn;
            VivoxService.Instance.LoggedOut += OnUserLoggedOut;
            VivoxService.Instance.ConnectionRecovered += OnConnectionRecovered;
            VivoxService.Instance.ConnectionRecovering += OnConnectionRecovering;
            VivoxService.Instance.ConnectionFailedToRecover += OnConnectionFailedToRecover;
            VivoxService.Instance.ChannelJoined += JoinedChannel;

            ///InitializeVivox();

            var loginOptions = new LoginOptions()
            {
                DisplayName = UnityServicesManager.Instance.AuthenticationController.PlayerName,
                ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.FivePerSecond
            };

            await VivoxService.Instance.LoginAsync(loginOptions);

        }

/*        public void SetVivoxInputMicrophone()
        {
            string defaultDevice = "Microphone (USB Audio Device)";

            VivoxService.Instance.SetActiveInputDeviceAsync(VivoxService.Instance.AvailableInputDevices.First(device => device.DeviceName == defaultDevice));
        }
*/

        private void JoinedChannel(string channel)
        {
            Debug.Log("Joined " + channel);

        }
        void OnUserLoggedIn()
        {
            Debug.Log("OnUserLoggedIn");
        }

        void OnUserLoggedOut()
        {
            Debug.Log("OnUserLoggedOut");

        }

        private void Update()
        {
#if XWIZARD_GAMES_VIVOX_SYSTEM

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (VivoxService.Instance.IsInputDeviceMuted)
                {
                    VivoxService.Instance.UnmuteInputDevice();
                    Debug.Log("Unmuted Input Device, you can now speak with other players in the room");

                }
                else
                {
                    VivoxService.Instance.MuteInputDevice();
                    Debug.Log("Muted Input Device");
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (VivoxService.Instance.IsOutputDeviceMuted)
                {
                    VivoxService.Instance.UnmuteOutputDevice();
                    Debug.Log("Unmuted Output Device, you can now hear other players in the room");

                }
                else
                {
                    VivoxService.Instance.MuteOutputDevice();
                    Debug.Log("Muted Output Device");
                }
            }

#endif
        }

        void OnConnectionRecovering()
        {
            Debug.Log("OnConnectionRecovering");

        }

        void OnConnectionRecovered()
        {
            Debug.Log("OnConnectionRecovered");

        }

        void OnConnectionFailedToRecover()
        {
            Debug.Log("OnConnectionFailedToRecover");

        }
        public Task JoinLobbyChannel(string joinKey)
        {
            Debug.Log(joinKey);
            return VivoxService.Instance.JoinGroupChannelAsync(joinKey, ChatCapability.TextAndAudio);
        }
#endif
    }
}
