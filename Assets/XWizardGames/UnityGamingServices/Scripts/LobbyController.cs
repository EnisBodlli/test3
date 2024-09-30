using PolymindGames;
using PolymindGames.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using XWizardGames.STP_MP;

#if XWIZARD_GAMES_LOBBY_SYSTEM
using Unity.Services.Lobbies;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
#endif
using UnityEngine;

namespace XWizardGames.UnityGamingServicesAddons.LobbyAddon
{
    public class LobbyController : ServiceController
    {
        [SerializeField]
        private List<string> m_Scenes = new List<string>();

        public List<string> SceneNames => m_Scenes;

#if XWIZARD_GAMES_LOBBY_SYSTEM

        public bool IsLocalPlayerHostOfCurrentLobby
        {
            get
            {
                if (m_CurrentLobby == null)
                {
                    return false;
                }

                return m_CurrentLobby.HostId.Equals(AuthenticationService.Instance.PlayerId);
            }
        }

        public const string PLAYER_NAME_KEY = "player_name";
        public const string ORIGINAL_HOST_ID_KEY = "original_host_id";
        public const string LOBBY_STATE = "lobby_state";
        public const string LOBBY_LANGUAGE = "english";
        public const string GAME_MODE_KEY = "game_mode";
        public const string RELAY_CODE_KEY = "relay_code";
        public const string VOICE_CHANNEL_KEY = "voice-channel";
        public const string MAP_KEY = "map";

        public Action<QueryResponse> OnLobbyListUpdated;
        public Action<Lobby> OnJoinedLobby;
        public Action<Lobby> OnCurrentLobbyInfoUpdated;
        public Action<Lobby> OnLobbyCreated;
        public Action OnLobbyLeft;
        public string RelayCode;
        private Lobby m_CurrentLobby;

        public Lobby CurrentLobby => m_CurrentLobby;

        private ILobbyEvents m_LobbyEvents;

        private float m_CurrentHeartBeatTimer = 0;

        [SerializeField]
        private float m_SendHeartBeatTimeInSeconds = 15;

        private bool m_IsCreatingOrJoining = false;
        private float m_CurrentLobbyUpdateTimer = 0;

        private void Update()
        {
            HandleLobbyHeartBeat();
            HandleLobbyPollForUpdates();
        }

        /// <summary>
        /// Refreshes the list of available lobbies.
        /// </summary>
        /// <param name="gameMode">The game mode to filter lobbies.</param>
        public async void RefreshLobbyList(GameModeType gameMode)
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                        new QueryFilter(QueryFilter.FieldOptions.S1, gameMode.ToString(), QueryFilter.OpOptions.EQ)
                    },
                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                Debug.Log("Lobbies found " + queryResponse.Results.Count);

                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log($"Found lobby! {lobby.Name} Max Players {lobby.MaxPlayers} Mode {lobby.Data[GAME_MODE_KEY].Value}");
                }

                OnLobbyListUpdated?.Invoke(queryResponse);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinRandomLobby()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    Order = new List<QueryOrder>
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                if (queryResponse.Results.Count > 0)
                {
                    // Choose a random lobby from the available lobbies
                    int randomIndex = UnityEngine.Random.Range(0, queryResponse.Results.Count);
                    Lobby randomLobby = queryResponse.Results[randomIndex];

                    // Join the selected lobby
                    JoinLobbyWithID(randomLobby.Id);
                }
                else
                {
                    Debug.LogError("No available lobbies found.");
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinLobbyWithID(string lobbyID)
        {
            Debug.Log("Joining Lobby with id " + lobbyID);
            try
            {
                JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
                {
                    Player = GetPlayer()
                };

                Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID, options);

                m_CurrentLobby = lobby;

                var callbacks = new LobbyEventCallbacks();
                callbacks.LobbyChanged += OnLobbyChanged;
                callbacks.KickedFromLobby += OnKickedFromLobby;
                callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

                m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(m_CurrentLobby.Id, callbacks);

                Debug.Log("Joined lobby " + lobby.Id);

                OnJoinedLobby?.Invoke(m_CurrentLobby);

                JoinedLobby();

                PrintPlayers(m_CurrentLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e.Message);
            }
        }

        /// <summary>
        /// Creates a Co-Op Zombies lobby.
        /// </summary>
        public async void CreateLobbyCoOpZombies()
        {
            await CreateLobby(GameModeType.FreeRoam, 4, false);
        }

        /// <summary>
        /// Creates a lobby with specified game mode, max players, and privacy setting.
        /// </summary>
        /// <param name="gameMode">The game mode of the lobby.</param>
        /// <param name="maxPlayers">Maximum number of players.</param>
        /// <param name="isPrivate">Whether the lobby is private.</param>
        public async Task<Lobby> CreateLobby(GameModeType gameMode, int maxPlayers, bool isPrivate = false)
        {
            if (m_IsCreatingOrJoining) return null;

            m_IsCreatingOrJoining = true;

            try
            {
                string lobbyName = $"{GetPlayer().Data[PLAYER_NAME_KEY].Value}'s room";

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
            {
                { ORIGINAL_HOST_ID_KEY, new DataObject(DataObject.VisibilityOptions.Public, UnityServicesManager.Instance.AuthenticationController.PlayerID) },
                { MAP_KEY, new DataObject(DataObject.VisibilityOptions.Public, m_Scenes[0]) },
                { VOICE_CHANNEL_KEY, new DataObject(DataObject.VisibilityOptions.Public, UnityServicesManager.Instance.AuthenticationController.PlayerID + "voice") },
                { GAME_MODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString(), DataObject.IndexOptions.S1) },
                { LOBBY_STATE, new DataObject(DataObject.VisibilityOptions.Public, MatchState.PreMatch.ToString(), DataObject.IndexOptions.S2) },
                { LOBBY_LANGUAGE, new DataObject(DataObject.VisibilityOptions.Public, "English US") }
            }
                };

                m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

                var callbacks = new LobbyEventCallbacks();
                callbacks.LobbyChanged += OnLobbyChanged;
                callbacks.KickedFromLobby += OnKickedFromLobby;
                callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

                m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(m_CurrentLobby.Id, callbacks);

                Debug.Log($"Created lobby ! {m_CurrentLobby.Name} Max Players {m_CurrentLobby.MaxPlayers} Lobby code {m_CurrentLobby.LobbyCode}");

                OnLobbyCreated?.Invoke(m_CurrentLobby);
                OnJoinedLobby?.Invoke(m_CurrentLobby);
                JoinedLobby();

                PrintPlayers(m_CurrentLobby);

                m_IsCreatingOrJoining = false;

                return m_CurrentLobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                m_IsCreatingOrJoining = false;
                return null;
            }
        }

        public void JoinRandomLobbyWithPrefab(string prefabName)
        {
            StartCoroutine(JoinRandomLobbyWithPrefabCoroutine(prefabName));
        }

        private IEnumerator JoinRandomLobbyWithPrefabCoroutine(string prefabName)
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            var queryTask = Lobbies.Instance.QueryLobbiesAsync(queryOptions);
            yield return new WaitUntil(() => queryTask.IsCompleted);

            if (queryTask.Result != null && queryTask.Result.Results.Count > 0)
            {
                int randomLobbyIndex = UnityEngine.Random.Range(0, queryTask.Result.Results.Count);
                Lobby selectedLobby = queryTask.Result.Results[randomLobbyIndex];

                JoinLobbyWithPrefab(selectedLobby.Id, prefabName);
            }
            else
            {
                Debug.LogError("No available lobbies found.");
            }
        }

        private async void JoinLobbyWithPrefab(string lobbyID, string prefabName)
        {
            try
            {
                JoinLobbyByIdOptions joinOptions = new JoinLobbyByIdOptions
                {
                    Player = new Unity.Services.Lobbies.Models.Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            { PLAYER_NAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, AuthenticationService.Instance.PlayerName) },
                            { "player_prefab", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, prefabName) }
                        }
                    }
                };

                Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID, joinOptions);
                m_CurrentLobby = lobby;

                var callbacks = new LobbyEventCallbacks();
                callbacks.LobbyChanged += OnLobbyChanged;
                callbacks.KickedFromLobby += OnKickedFromLobby;
                callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;

                m_LobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(m_CurrentLobby.Id, callbacks);

                OnJoinedLobby?.Invoke(m_CurrentLobby);

                JoinedLobby();

                PrintPlayers(m_CurrentLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (m_CurrentLobby == null) return;
            if (HasSession) return;

            try
            {
                m_CurrentLobbyUpdateTimer -= Time.deltaTime;
                if (m_CurrentLobbyUpdateTimer <= 0)
                {
                    m_CurrentLobbyUpdateTimer = 1.2f;

                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(m_CurrentLobby.Id);

                    bool isLocalPlayerInLobby = false;

                    foreach (Unity.Services.Lobbies.Models.Player p in lobby.Players)
                    {
                        if (p.Id.Equals(UnityServicesManager.Instance.AuthenticationController.PlayerID))
                        {
                            isLocalPlayerInLobby = true;
                        }
                    }

                    if (!isLocalPlayerInLobby)
                    {
                        return;
                    }

                    m_CurrentLobby = lobby;
                    OnCurrentLobbyInfoUpdated?.Invoke(m_CurrentLobby);

                    if (m_CurrentLobby != null)
                    {
                        if (m_CurrentLobby.Data.TryGetValue(RELAY_CODE_KEY, out DataObject data))
                        {
                            string relayCode = data.Value;
                            if (string.IsNullOrEmpty(relayCode)) return;

                            if (!IsLocalPlayerHostOfCurrentLobby && !HasSession)
                            {
                                RelayCode = relayCode;
                                HasSession = true;
                                LoadLevel(m_CurrentLobby.Data[MAP_KEY].Value);
                            }
                        }
                    }
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.LobbyNotFound)
                {
                    m_CurrentLobby = null;
                }
                Debug.Log(e.Message);
            }
        }

        private async void HandleLobbyHeartBeat()
        {
            if (!IsLocalPlayerHostOfCurrentLobby) return;
            if (m_CurrentLobby == null) return;

            try
            {
                m_CurrentHeartBeatTimer -= Time.deltaTime;
                if (m_CurrentHeartBeatTimer <= 0)
                {
                    m_CurrentHeartBeatTimer = m_SendHeartBeatTimeInSeconds;

                    await LobbyService.Instance.SendHeartbeatPingAsync(m_CurrentLobby.Id);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void UpdateRelayJoinCode(string relayCode)
        {
            if (m_CurrentLobby == null)
                return;

            m_CurrentLobby = await Lobbies.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                },
            });
        }

        public async void ChangeMap(int index)
        {
            m_CurrentLobby = await Lobbies.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { MAP_KEY, new DataObject(DataObject.VisibilityOptions.Public, m_Scenes[index]) }
                },
            });
        }

        public async Task<bool> LeaveLobby()
        {
            if (m_CurrentLobby == null) return true;

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(m_CurrentLobby.Id, AuthenticationService.Instance.PlayerId);
                m_CurrentLobby = null;
                m_LobbyEvents = null;

                OnLobbyLeft?.Invoke();

                Debug.Log("Left lobby");
                return true; // Return true if the operation is successful
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false; // Return false in case of failure
            }
        }

        public void StartGame()
        {
            HasSession = true;
            LoadLevel(m_CurrentLobby.Data[MAP_KEY].Value);
        }

        private bool m_IsLoading;

        [SerializeField, Range(0f, 10f)]
        private float m_LoadDelay = 2;

        public bool HasSession = false;

        private void LoadLevel(string sceneName)
        {
            if (!m_IsLoading)
                StartCoroutine(C_LoadLevel(sceneName));
        }

        private IEnumerator C_LoadLevel(string sceneName)
        {
            m_IsLoading = true;
            FadeScreenUI.Instance.Fade(true, 0.2f);

            yield return new WaitForSeconds(m_LoadDelay);

            LevelManager.LoadScene(sceneName);

            yield return new WaitUntil(() => LevelManager.IsLoading == false);

            SessionValidator session = FindObjectOfType<SessionValidator>();

            if (session != null)
                session.StartRelayHostOrClient();
        }

#if XWIZARD_GAMES_VIVOX_SYSTEM
        [SerializeField] private bool m_ConnectToVivox = true;
#endif

        private void JoinedLobby()
        {
#if XWIZARD_GAMES_VIVOX_SYSTEM
            if (m_ConnectToVivox && m_CurrentLobby.Data.TryGetValue(VOICE_CHANNEL_KEY, out DataObject data))
            {
                Debug.Log("Joining Voice");

                UnityServicesManager.Instance.VivoxController.JoinLobbyChannel(data.Value);
            }
#endif
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
        {
            switch (state)
            {
                case LobbyEventConnectionState.Unsubscribed:
                    // Update the UI if necessary, as the subscription has been stopped.
                    break;
                case LobbyEventConnectionState.Subscribing:
                    // Update the UI if necessary, while waiting to be subscribed.
                    break;
                case LobbyEventConnectionState.Subscribed:
                    // Update the UI if necessary, to show subscription is working.
                    break;
                case LobbyEventConnectionState.Unsynced:
                    // Update the UI to show connection problems. Lobby will attempt to reconnect automatically.
                    break;
                case LobbyEventConnectionState.Error:
                    // Update the UI to show the connection has errored. Lobby will not attempt to reconnect as something has gone wrong.
                    break;
            }
        }

        private void OnKickedFromLobby()
        {
            Debug.Log("OnKickedFromLobby method called");

            m_CurrentLobby = null;
            m_LobbyEvents = null;
        }

        public void PrintPlayers(Lobby lobby)
        {
            Debug.Log("Players in lobby " + lobby.Name);
            foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
            {
                Debug.Log(player.Id + " " + player.Data[PLAYER_NAME_KEY].Value + " " + lobby.Data[GAME_MODE_KEY].Value);
            }
        }

        private void OnLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                Debug.Log("LobbyDeleted");
                changes.ApplyToLobby(m_CurrentLobby);
                // Handle lobby being deleted
                // Calling changes.ApplyToLobby will log a warning and do nothing
            }
            else
            {
                changes.ApplyToLobby(m_CurrentLobby);
            }
            // Refresh the UI in some way
        }

        private Unity.Services.Lobbies.Models.Player GetPlayer()
        {
            return new Unity.Services.Lobbies.Models.Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { PLAYER_NAME_KEY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, UnityServicesManager.Instance.AuthenticationController.PlayerName) },
                }
            };
        }

        public override void Initialize()
        {
            Debug.Log("Lobby Controller Initialized");
        }

#endif // Closing the #if XWIZARD_GAMES_LOBBY_SYSTEM block
    }

    public enum GameModeType
    {
        FreeRoam,
    }

    public enum MatchState
    {
        PreMatch,
        OnGoing
    }
}
