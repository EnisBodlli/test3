using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using XWizardGames.UnityGamingServicesAddons; // Namespace for UnityServicesManager and other services
using XWizardGames.UnityGamingServicesAddons.LobbyAddon; // Namespace for LobbyController

/// <summary>
/// Handles the process of joining a random lobby for monsters and managing scene transitions.
/// </summary>
public class RandomLobbyJoiner : NetworkBehaviour
{
    [Header("UI References")]
    [Tooltip("UI element to display when no rooms are available.")]
    public GameObject noRoomsAvailableUI;

    [Header("Scene Management")]
    [Tooltip("Name of the scene to load after successfully joining a room.")]
    public string sceneToLoadAfterJoining = "GameScene"; // Replace with your actual scene name

    [Header("Monster Prefab Configuration")]
    [Tooltip("Name of the monster prefab to set in PlayerPrefs.")]
    [SerializeField] private string monsterPrefabNameToSet = "EnemyPlayer"; // Replace with your actual monster prefab name

    [Header("Script Control")]
    public bool enableScript = true;  // Inspector checkbox

    [Header("Spawner References")]
    [Tooltip("Reference to the PlayerMonsterSpawner script.")]
    public PlayerMonsterSpawner monsterSpawner; // Assign via Inspector

    void Awake()
    {
        if (!enableScript) return; // If the script is "disabled", return early
        Debug.Log("RandomLobbyJoiner: Awake called. Setting 'MonsterPrefabName' in PlayerPrefs.");

        // Set the "MonsterPrefabName" in PlayerPrefs
        PlayerPrefs.SetString("MonsterPrefabName", monsterPrefabNameToSet);
        PlayerPrefs.Save();
        Debug.Log($"RandomLobbyJoiner: Set 'MonsterPrefabName' to '{monsterPrefabNameToSet}' in PlayerPrefs.");

        // Verify that the key is set
        if (PlayerPrefs.HasKey("MonsterPrefabName"))
        {
            string retrievedName = PlayerPrefs.GetString("MonsterPrefabName");
            Debug.Log($"RandomLobbyJoiner: Verified 'MonsterPrefabName' is set to '{retrievedName}'.");
        }
        else
        {
            Debug.LogError("RandomLobbyJoiner Error: 'MonsterPrefabName' was not set correctly in PlayerPrefs.");
        }
    }

    public void JoinRandomRoom()
    {
        if (!enableScript) return; // If the script is "disabled", return early

        if (!isServer)
        {
            Debug.LogError("RandomLobbyJoiner Error: JoinRandomRoom should only be called on the server.");
            return;
        }

        Debug.Log("RandomLobbyJoiner: Initiating JoinRandomRoom process.");
        StartCoroutine(JoinRandomRoomCoroutine());
    }

    private IEnumerator JoinRandomRoomCoroutine()
    {
        if (!enableScript) yield break; // If the script is "disabled", stop the coroutine

        Debug.Log("RandomLobbyJoiner: Starting JoinRandomRoomCoroutine.");

        // Safely reference the LobbyController from the UnityServicesManager instance
        LobbyController lobbyController = UnityServicesManager.Instance?.LobbyController;
        Debug.Log("RandomLobbyJoiner: Retrieved LobbyController from UnityServicesManager.");

        if (lobbyController == null)
        {
            Debug.LogError("RandomLobbyJoiner Error: LobbyController is not available.");
            yield break;
        }

        // Attempt to join a random lobby with the specified prefab
        string actualPrefabName = PlayerPrefs.GetString("MonsterPrefabName", "DefaultEnemyPrefab"); // Ensure default value
        lobbyController.JoinRandomLobbyWithPrefab(prefabName: actualPrefabName);
        Debug.Log("RandomLobbyJoiner: JoinRandomLobbyWithPrefab called.");

        // Wait until the lobby is successfully joined or a timeout occurs
        float timeout = 10f; // 10 seconds timeout
        float timer = 0f;
        while (lobbyController.CurrentLobby == null && timer < timeout)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        if (lobbyController.CurrentLobby != null)
        {
            Debug.Log("RandomLobbyJoiner: Successfully joined a random room.");
            Debug.Log($"RandomLobbyJoiner: Loading scene '{sceneToLoadAfterJoining}' after joining the room.");

            // Optionally, perform server-side operations before loading the scene
            if (monsterSpawner != null)
            {
                monsterSpawner.SpawnMonster(monsterPrefabNameToSet);
            }
            else
            {
                Debug.LogError("RandomLobbyJoiner Error: MonsterSpawner reference is not set.");
            }

            // Load the specified scene after successfully joining the room
            NetworkManager.singleton.ServerChangeScene(sceneToLoadAfterJoining);
            Debug.Log($"RandomLobbyJoiner: Scene '{sceneToLoadAfterJoining}' loading initiated.");
        }
        else
        {
            Debug.LogError("RandomLobbyJoiner Error: Failed to join a random lobby within timeout.");
            ActivateNoRoomsAvailableUI();
        }

        Debug.Log("RandomLobbyJoiner: JoinRandomRoomCoroutine completed.");
    }

    private void ActivateNoRoomsAvailableUI()
    {
        if (!enableScript) return; // If the script is "disabled", return early

        Debug.Log("RandomLobbyJoiner: Activating 'No Rooms Available' UI.");
        if (noRoomsAvailableUI != null)
        {
            noRoomsAvailableUI.SetActive(true);
            Debug.Log("RandomLobbyJoiner: 'No Rooms Available' UI is now active.");
        }
        else
        {
            Debug.LogWarning("RandomLobbyJoiner Warning: noRoomsAvailableUI is not assigned in the Inspector.");
        }
    }
}
