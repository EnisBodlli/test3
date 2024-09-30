using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Utp;
#if XWIZARD_GAMES_RELAY_SYSTEM
using Unity.Services.Relay.Models;
#endif

namespace XWizardGames.STP_MP
{
    public class RelayNetworkManager : STP_MP_NetworkManager
    {
        public UtpTransport UtpTransport => utpTransport;
        private UtpTransport utpTransport;
        public GameObject playerPrefab;

        public string relayJoinCode = "";

        public override void Awake()
        {
            base.Awake();
            utpTransport = GetComponent<UtpTransport>();

            string[] args = System.Environment.GetCommandLineArgs();
            for (int key = 0; key < args.Length; key++)
            {
                if (args[key] == "-port")
                {
                    if (key + 1 < args.Length)
                    {
                        string value = args[key + 1];
                        try
                        {
                            utpTransport.Port = ushort.Parse(value);
                        }
                        catch
                        {
                            UtpLog.Warning($"Unable to parse {value} into transport Port");
                        }
                    }
                }
            }
        }

        public ushort GetPort()
        {
            return utpTransport.Port;
        }

        public bool IsRelayEnabled()
        {
            return utpTransport.useRelay;
        }

        public void StartStandardServer()
        {
            utpTransport.useRelay = false;
            StartServer();
        }

        public void StartStandardHost()
        {
            utpTransport.useRelay = false;
            StartHost();
        }

        public void GetRelayRegions(Action<List<Region>> onSuccess, Action onFailure)
        {
            utpTransport.GetRelayRegions(onSuccess, onFailure);
        }

        public Action<string> OnJoinRoomCodeUpdated;

        public void StartRelayHost(int maxPlayers, string regionId = null)
        {
            utpTransport.useRelay = true;
            utpTransport.AllocateRelayServer(maxPlayers, regionId,
            (string joinCode) =>
            {
                relayJoinCode = joinCode;
                OnJoinRoomCodeUpdated?.Invoke(relayJoinCode);
                StartHost();
            },
            () =>
            {
                UtpLog.Error($"Failed to start a Relay host.");
            });
        }

        public void StartRelayServer(int maxPlayers, string regionId = null)
        {
            utpTransport.useRelay = true;
            utpTransport.AllocateRelayServer(maxPlayers, regionId,
            (string joinCode) =>
            {
                relayJoinCode = joinCode;
                OnJoinRoomCodeUpdated?.Invoke(relayJoinCode);
                StartServer();
            },
            () =>
            {
                UtpLog.Error($"Failed to start a Relay server.");
            });
        }

        public void JoinStandardServer()
        {
            utpTransport.useRelay = false;
            StartClient();
        }

        public void JoinRelayServer()
        {
            utpTransport.useRelay = true;
            utpTransport.ConfigureClientWithJoinCode(relayJoinCode,
            () =>
            {
                StartClient();
            },
            () =>
            {
                UtpLog.Error($"Failed to join Relay server.");
            });
        }

        private void SpawnPlayerAtMainSpawn(NetworkConnectionToClient conn)
        {
            GameObject spawnPoint = GameObject.Find("MainSpawn");
            if (spawnPoint != null)
            {
                Vector3 spawnPosition = spawnPoint.transform.position;
                GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                NetworkServer.AddPlayerForConnection(conn, player);
            }
            else
            {
                Debug.LogError("No GameObject named 'MainSpawn' found in the scene.");
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            SpawnPlayerAtMainSpawn(conn);
        }
    }
}
