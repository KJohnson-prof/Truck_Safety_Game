using System.Collections;
using UnityEngine;

public class SimpleCarSpawner : MonoBehaviour
{
    [Header("Car Prefabs")]
    public GameObject[] carPrefabs;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Movement")]
    public Vector3 moveDirection = Vector3.forward;
    public float travelDistance = 60f;
    public float minSpeed = 5f;
    public float maxSpeed = 12f;

    [Header("Spawn Timing")]
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 15f;

    void Start()
    {
        if (carPrefabs == null || carPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogError("SimpleCarSpawner: missing prefab or spawn point!");
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            SpawnCar();
        }
    }

    void SpawnCar()
    {
        if (carPrefabs == null || carPrefabs.Length == 0) return;
        
        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)]; // ← random pick
        GameObject car = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        
        SimpleCarMover mover = car.GetComponent<SimpleCarMover>();
        if (mover == null)
            mover = car.AddComponent<SimpleCarMover>();

        mover.moveDirection = moveDirection;
        mover.travelDistance = travelDistance;
        mover.speed = Random.Range(minSpeed, maxSpeed);
    }

    void OnDrawGizmos()
    {
        if (spawnPoint == null) return;

        Gizmos.color = Color.red; // red sline
        Gizmos.DrawSphere(spawnPoint.position, 0.5f);

        Gizmos.color = new Color(1f, 0.5f, 0f); // orange line
        Vector3 endPoint = spawnPoint.position + (moveDirection.normalized * travelDistance);
        Gizmos.DrawLine(spawnPoint.position, endPoint);
        Gizmos.DrawSphere(endPoint, 0.5f);
    }
}