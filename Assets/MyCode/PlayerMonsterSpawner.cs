using System.Collections.Generic;
using UnityEngine;
using Mirror; // Required for networked spawning

public class PlayerMonsterSpawner : NetworkBehaviour
{
    [SerializeField] private Transform spawnPoint; // The location where the monster will spawn
    [SerializeField] private List<GameObject> spawnedMonsters = new List<GameObject>(); // List to store references to all spawned monsters

    void Awake()
    {
        Debug.Log("MonsterSpawner: Awake called.");
    }

    void Start()
    {
        Debug.Log("MonsterSpawner: Start called.");
        // Removed automatic spawning to prevent monsters from spawning on scene load
    }

    /// <summary>
    /// Public method to spawn a monster. Call this method when you want to spawn a monster.
    /// </summary>
    /// <param name="monsterPrefabName">Name of the monster prefab to spawn.</param>
    [Server]
    public void SpawnMonster(string monsterPrefabName)
    {
        if (string.IsNullOrEmpty(monsterPrefabName))
        {
            Debug.LogError("MonsterSpawner Error: MonsterPrefabName is null or empty.");
            return;
        }

        Debug.Log($"MonsterSpawner: Attempting to load prefab '{monsterPrefabName}'.");

        // Load the monster prefab from the Resources folder or another appropriate location
        GameObject monsterPrefab = Resources.Load<GameObject>(monsterPrefabName);

        if (monsterPrefab != null)
        {
            Debug.Log($"MonsterSpawner: Successfully loaded monster prefab '{monsterPrefabName}'. Instantiating monster.");

            // Instantiate the monster at the spawn point
            GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"MonsterSpawner: Instantiated monster '{monsterPrefabName}' at position {spawnPoint.position}.");

            // Spawn the monster across the network
            NetworkServer.Spawn(spawnedMonster);
            Debug.Log("MonsterSpawner: Monster spawned and synchronized across all clients.");

            // Add the spawned monster to the list
            spawnedMonsters.Add(spawnedMonster);
            Debug.Log($"MonsterSpawner: Added spawned monster to the list. Total monsters spawned: {spawnedMonsters.Count}.");
        }
        else
        {
            Debug.LogError($"MonsterSpawner Error: Monster prefab '{monsterPrefabName}' not found in Resources.");
        }
    }
}
