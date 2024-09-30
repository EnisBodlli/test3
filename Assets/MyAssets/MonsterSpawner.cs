using System.Collections;
using System.Collections.Generic;  // Needed for List<>
using UnityEngine;
using Mirror;
using UnityEngine.AI; // Required for NavMesh related classes
using PolymindGames.WorldManagement; // Ensure this namespace is correct for WorldManager and TimeOfDay

public class MonsterSpawner : MonoBehaviour
{
    public WorldManager worldManager; // Reference to the WorldManager script
    public string monsterPrefabName = "DefaultMonster"; // Name of the monster prefab in the Resources folder
    public GameObject[] spawnPoints; // Array of spawn points
    public float spawnInterval = 10f; // Interval in seconds for spawning monsters
    public float moveSpeed = 3.5f; // Speed of the monsters controlled by the spawner
    public float wanderRadius = 10f; // Radius within which the monsters will wander
    public float wanderCooldown = 5f; // Cooldown time for wander behavior
    public float attackDistance = 1f; // Distance at which monsters will attack
    public float attackCooldown = 1f; // Cooldown time between attacks
    public float minDamage = 2; // Minimum attack damage
    public float maxDamage = 4; // Maximum attack damage

    private List<GameObject> spawnedMonsters = new List<GameObject>(); // Store all spawned monsters
    private Coroutine spawnCoroutine;

    void Start()
    {
        if (worldManager == null)
        {
            Debug.LogError("WorldManager not assigned.");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }

        StartCoroutine(CheckForNightTime());
    }

    private IEnumerator CheckForNightTime()
    {
        while (true)
        {
            if (worldManager.GetTimeOfDay() == TimeOfDay.Night && spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnMonsters());
            }
            else if (worldManager.GetTimeOfDay() == TimeOfDay.Day && spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
            yield return new WaitForSeconds(1f); // Check every second
        }
    }

    private IEnumerator SpawnMonsters()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        if (!NetworkServer.active) return; // Ensure this only runs on the server

        // Load the monster prefab from the Resources folder
        GameObject monsterPrefab = Resources.Load<GameObject>(monsterPrefabName);

        if (monsterPrefab == null)
        {
            Debug.LogError("Monster prefab not found in Resources: " + monsterPrefabName);
            return;
        }

        // Choose a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].transform.position;

        // Instantiate the monster at the selected spawn point
        GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);

        // For multiplayer games, ensure the monster is spawned on the network
        NetworkServer.Spawn(spawnedMonster);

        // Add to the list of controlled monsters
        spawnedMonsters.Add(spawnedMonster);
    }

    void Update()
    {
        if (!NetworkServer.active) return; // Ensure this only runs on the server

        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster == null) continue;

            NavMeshAgent agent = monster.GetComponent<NavMeshAgent>();

            if (agent == null)
            {
                Debug.LogError("Spawned monster does not have a NavMeshAgent component.");
                continue;
            }

            // Example AI behavior: make the monster wander if it has no target
            Wander(monster, agent);
        }
    }

    private void Wander(GameObject monster, NavMeshAgent agent)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Generate a random point within the wander radius
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += monster.transform.position;

            NavMeshHit hit;
            // Find a valid point on the NavMesh closest to the random direction
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    // Add other centralized AI behaviors such as FindTarget and Attack here
}
