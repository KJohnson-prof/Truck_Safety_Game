using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRandomPedestrianSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Pedestrian Prefab")]
    public GameObject pedestrianPrefab;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 30f;   // Min 30 seconds between spawns
    public float maxSpawnInterval = 60f;   // Max 60 seconds between spawns

    [Header("Pedestrian Spawn Position")]
    public Transform spawnPoint;            // Where pedestrians spawn

    [Header("Crossing Direction & Distance")]
    public Vector3 crossDirection = Vector3.forward;  // Direction they walk
    public float crossDistance = 20f;       // Distance they travel

    [Header("Speed Range")]
    public float minSpeed = 1f;
    public float maxSpeed = 1.8f;

    [Header("Truck Collision")]
    public string truckTag = "Truck";

    // ─────────────────────────────────────────────
    // Private State
    // ─────────────────────────────────────────────

    private List<GameObject> activePedestrians = new List<GameObject>();

    // ─────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────

    void Start()
    {
        if (pedestrianPrefab == null)
        {
            Debug.LogError("SimpleRandomPedestrianSpawner: No pedestrian prefab assigned!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("SimpleRandomPedestrianSpawner: No spawn point assigned!");
            return;
        }

        StartCoroutine(SpawnRoutine());
        StartCoroutine(CleanupRoutine());
    }

    // ───────────────────────────────────��─────────
    // Spawning
    // ─────────────────────────────────────────────

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for random interval (30-60 seconds)
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Spawn a pedestrian
            SpawnPedestrian();
        }
    }

    void SpawnPedestrian()
    {
        if (pedestrianPrefab == null || spawnPoint == null) return;

        // Instantiate at spawn point
        GameObject ped = Instantiate(pedestrianPrefab, spawnPoint.position, Quaternion.identity);

        // Configure the pedestrian
        SimplePedestrianCrosser crosser = ped.GetComponent<SimplePedestrianCrosser>();
        if (crosser != null)
        {
            crosser.crossDirection = crossDirection.normalized;
            crosser.crossDistance = crossDistance;
            crosser.minSpeed = minSpeed;
            crosser.maxSpeed = maxSpeed;
            crosser.truckTag = truckTag;
        }

        activePedestrians.Add(ped);
        Debug.Log($"Spawned pedestrian at spawn point | Active: {activePedestrians.Count}");
    }

    // ─────────────────────────────────��───────────
    // Cleanup
    // ─────────────────────────────────────────────

    IEnumerator CleanupRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            activePedestrians.RemoveAll(p => p == null);
        }
    }

    // ─────────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────────

    void OnDrawGizmos()
    {
        if (spawnPoint == null) return;

        // Draw spawn point
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPoint.position, 0.5f);

        // Draw crossing path
        Gizmos.color = Color.cyan;
        Vector3 endPoint = spawnPoint.position + (crossDirection.normalized * crossDistance);
        Gizmos.DrawLine(spawnPoint.position, endPoint);
        Gizmos.DrawSphere(endPoint, 0.5f);
    }
}