using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnZone
{
    public Transform center;
    public float     radius      = 5f;
    public bool      isCrosswalk = false;

    public Transform[] sidewalkWaypoints;
    public Transform[] otherSideWaypoints;

    [HideInInspector] public float cooldownUntil = 0f;
}

public class PedestrianSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Truck Reference")]
    public Transform truck;

    [Header("Pedestrian Prefabs")]
    public GameObject[] pedestrianPrefabs;

    [Header("Static Spawn Zones (optional, for non-endless scenes)")]
    public SpawnZone[] spawnZones;

    [Header("Traffic Light")]
    public TrafficLightController trafficLight;

    [Header("Spawn Settings")]
    public int   maxPedestrians     = 10;
    public float spawnInterval      = 2f;
    public float minSpawnDistance   = 10f;
    public float spawnAheadDistance = 40f;
    public float despawnDistance    = 55f;
    public float ruleBreakChance    = 0.25f;

    [Header("Crosswalk Cross Chance")]
    [Range(0f,1f)]
    public float crosswalkCrossChance = 0.7f;
    [Range(0f,1f)]
    public float sidewalkCrossChance  = 0.2f;

    [Header("Zone Cooldown")]
    public float zoneCooldown = 5f;

    // ─────────────────────────────────────────────
    // Private State
    // ─────────────────────────────────────────────

    private List<SpawnZone> spawnZoneList = new List<SpawnZone>();
    private List<GameObject> activePedestrians = new List<GameObject>();

    // ─────────────────────────────────────────────
    // Public Registration API (called by SectionSpawnZoneRegistrar)
    // ─────────────────────────────────────────────

    public void RegisterZones(SpawnZone[] zones)
    {
        if (zones == null) return;
        foreach (var z in zones)
            if (z != null && !spawnZoneList.Contains(z))
                spawnZoneList.Add(z);
    }

    public void UnregisterZones(SpawnZone[] zones)
    {
        if (zones == null) return;
        foreach (var z in zones)
            spawnZoneList.Remove(z);
    }

    // ─────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────

    void Start()
    {
        if (truck == null)
        {
            Debug.LogWarning("PedestrianSpawner: No truck assigned!");
            return;
        }

        // Add any static zones set in Inspector
        if (spawnZones != null)
            foreach (var z in spawnZones)
                if (z != null) spawnZoneList.Add(z);

        StartCoroutine(SpawnRoutine());
        StartCoroutine(DespawnRoutine());
    }

    // ─────────────────────────────────────────────
    // Spawning
    // ─────────────────────────────────────────────

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            activePedestrians.RemoveAll(p => p == null);

            if (activePedestrians.Count < maxPedestrians)
                TrySpawnPedestrian();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void TrySpawnPedestrian()
    {
        if (pedestrianPrefabs == null || pedestrianPrefabs.Length == 0)
        {
            Debug.LogWarning("PedestrianSpawner: No prefabs assigned!");
            return;
        }

        if (spawnZoneList.Count == 0) return;

        List<int> validIndices = new List<int>();

        for (int i = 0; i < spawnZoneList.Count; i++)
        {
            SpawnZone zone = spawnZoneList[i];
            if (zone == null || zone.center == null) continue;

            Vector3 toZone     = zone.center.position - truck.position;
            float   distance   = toZone.magnitude;
            float   dotForward = Vector3.Dot(truck.forward, toZone.normalized);

            if (dotForward > 0.2f && distance < spawnAheadDistance && distance > minSpawnDistance
                && Time.time >= zone.cooldownUntil)
                validIndices.Add(i);
        }

        if (validIndices.Count == 0) return;

        int       chosenIndex = validIndices[Random.Range(0, validIndices.Count)];
        SpawnZone chosenZone  = spawnZoneList[chosenIndex];

        Vector3 spawnPos;
        if (!GetRandomNavMeshPointInZone(chosenZone.center.position, chosenZone.radius, out spawnPos))
        {
            Debug.LogWarning($"PedestrianSpawner: Could not find NavMesh point in zone {chosenIndex}");
            return;
        }

        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];
        GameObject ped    = Instantiate(prefab, spawnPos, Quaternion.identity);

        PedestrianController controller = ped.GetComponent<PedestrianController>();
        if (controller != null)
        {
            controller.sidewalkWaypoints  = chosenZone.sidewalkWaypoints;
            controller.otherSideWaypoints = chosenZone.otherSideWaypoints;
            controller.trafficLight       = trafficLight;
            controller.crossChance        = chosenZone.isCrosswalk ? crosswalkCrossChance : sidewalkCrossChance;
            controller.isRuleBreaker      = Random.value < ruleBreakChance;
        }

        activePedestrians.Add(ped);
        spawnZoneList[chosenIndex].cooldownUntil = Time.time + zoneCooldown;

        Debug.Log($"Spawned pedestrian at zone {chosenIndex} | Active: {activePedestrians.Count}");
    }

    bool GetRandomNavMeshPointInZone(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomDir  = Random.insideUnitSphere * radius;
            randomDir         += center;
            randomDir.y        = center.y;

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomDir, out hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = center;
        return false;
    }

    // ─────────────────────────────────────────────
    // Despawning
    // ─────────────────────────────────────────────

    IEnumerator DespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            for (int i = activePedestrians.Count - 1; i >= 0; i--)
            {
                GameObject ped = activePedestrians[i];

                if (ped == null)
                {
                    activePedestrians.RemoveAt(i);
                    continue;
                }

                Vector3 toPed      = ped.transform.position - truck.position;
                float   distance   = toPed.magnitude;
                float   dotForward = Vector3.Dot(truck.forward, toPed.normalized);

                if (dotForward < -0.3f && distance > despawnDistance)
                {
                    activePedestrians.RemoveAt(i);
                    Destroy(ped);
                    Debug.Log("Despawned pedestrian (out of range)");
                }
            }
        }
    }
   
    void OnDrawGizmos()
    {
        if (spawnZoneList != null)
        {
            foreach (SpawnZone zone in spawnZoneList)
            {
                if (zone == null || zone.center == null) continue;

                Gizmos.color = zone.isCrosswalk
                    ? new Color(0f, 1f, 1f, 0.25f)
                    : new Color(0f, 1f, 0f, 0.2f);

                Gizmos.DrawSphere(zone.center.position, zone.radius);
                Gizmos.DrawWireSphere(zone.center.position, zone.radius);
            }
        }

        if (truck != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.08f);
            Gizmos.DrawSphere(truck.position + truck.forward * (spawnAheadDistance * 0.5f), spawnAheadDistance * 0.5f);

            Gizmos.color = new Color(1f, 0f, 0f, 0.08f);
            Gizmos.DrawWireSphere(truck.position, despawnDistance);

            Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
            Gizmos.DrawWireSphere(truck.position, minSpawnDistance);
        }
    }
}