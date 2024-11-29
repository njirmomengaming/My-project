using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class PowerUpSpawner : AttributesSync
{
    public GameObject[] powerUpPrefabs; // Array of power-up prefabs (e.g., ShieldPowerUp, HealthPowerUp)
    public float spawnInterval = 15f;    // Time interval for spawning power-ups
    public GameObject powerUpSpawnPointParent; // Parent object holding all spawn points

    private Transform[] spawnPoints;      // Array to hold spawn point transforms
    private Dictionary<Transform, GameObject> spawnedPowerUps = new Dictionary<Transform, GameObject>(); // Track spawned power-ups at spawn points
    private HashSet<string> spawnedPowerUpTypes = new HashSet<string>(); // Track the types of power-ups that have been spawned

    private void Start()
    {
        // Ensure the parent object is assigned
        powerUpSpawnPointParent = GameObject.Find("PowerUpSpawner");

        if (powerUpSpawnPointParent != null)
        {
            // Get all child transforms (spawn points) from the parent
            spawnPoints = powerUpSpawnPointParent.GetComponentsInChildren<Transform>();
            spawnPoints = System.Array.FindAll(spawnPoints, t => t != powerUpSpawnPointParent.transform); // Exclude the parent object itself
        }
        else
        {
            Debug.LogError("PowerUpSpawnPoint parent object is not assigned!");
        }

        BroadcastRemoteMethod("StartSpawnPowerUpRoutine");
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        // Wait a random time before starting to spawn power-ups
        yield return new WaitForSeconds(Random.Range(0, spawnInterval / 2));

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check and spawn missing power-ups
            BroadcastRemoteMethod("CheckAndSpawnPowerUps");
        }
    }

    [SynchronizableMethod]
    private void CheckAndSpawnPowerUps()
    {
        // Iterate through the power-ups to check if each is already in the scene
        foreach (GameObject powerUpPrefab in powerUpPrefabs)
        {
            string powerUpName = powerUpPrefab.name;

            // If this power-up type has already been spawned, skip it
            // if (spawnedPowerUpTypes.Contains(powerUpName))
            // {
            //     continue; // Skip spawning this power-up if it's already in the scene
            // }

            if (GameObject.Find("HealthPowerUp") != null && GameObject.Find("ShieldPowerUp") != null)
            {
                continue;
            }

            // If the power-up is not found in the scene, spawn it
            foreach (Transform spawnPoint in spawnPoints)
            {
                // If this spawn point doesn't have a power-up yet, spawn one
                if (spawnPoint != null && !spawnedPowerUps.ContainsKey(spawnPoint))
                {
                    SpawnPowerUp(powerUpPrefab, spawnPoint.position, spawnPoint);
                    break; // Exit after spawning a power-up at the first available point
                }
            }
        }
    }

    [SynchronizableMethod]
    private void OnSpawnPositionChanged(Vector3 newPosition)
    {
        // Spawn a power-up at the given position (only if not already spawned)
        foreach (GameObject powerUpPrefab in powerUpPrefabs)
        {
            string powerUpName = powerUpPrefab.name;

            // Only spawn if this power-up hasn't been spawned already
            if (!spawnedPowerUpTypes.Contains(powerUpName))
            {
                SpawnPowerUp(powerUpPrefab, newPosition, null);
            }
        }
    }

    private void SpawnPowerUp(GameObject powerUpPrefab, Vector3 position, Transform spawnPoint)
    {
        if (powerUpPrefab == null)
        {
            Debug.LogWarning("Power-up prefab is not assigned!");
            return;
        }

        GameObject spawnedPowerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);
        spawnedPowerUp.name = powerUpPrefab.name; // Ensure the name matches for checking

        if (spawnPoint != null)
        {
            // Track the spawned power-up at the spawn point
            spawnedPowerUps[spawnPoint] = spawnedPowerUp;
        }

        // Track the type of the spawned power-up to prevent spawning duplicates
        spawnedPowerUpTypes.Add(powerUpPrefab.name);

        Debug.LogWarning("Power-up " + spawnedPowerUp.name + " spawned at position: " + position);
    }

    [SynchronizableMethod]
    private void StartSpawnPowerUpRoutine()
    {
        StartCoroutine(SpawnPowerUpRoutine());
    }
}
