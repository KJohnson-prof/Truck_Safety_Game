using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector Settings
    // ─────────────────────────────────────────────

    [Header("Sidewalk Wander Settings")]
    public float wanderRadius = 10f;
    public float minWanderWait = 2f;
    public float maxWanderWait = 5f;

    [Header("Crossing Settings")]
    public Transform[] sidewalkWaypoints;   // Waypoint_1, 2, 3 on right sidewalk
    public Transform[] otherSideWaypoints;  // Waypoint_1, 2, 3 on left sidewalk
    public float crossChance = 0.3f;
    public bool isRuleBreaker = false;
    public float minWaitBeforeCross = 1f;
    public float maxWaitBeforeCross = 4f;

    [Header("Traffic Light")]
    public TrafficLightController trafficLight;

    [Header("Speed Settings")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.5f;

    [Header("Truck Collision")]
    public string truckTag = "Truck";

    [Header("Death Animation")]
    public string deathAnimTrigger = "Die";
    public float deathAnimDuration = 2f;


    // ─────────────────────────────────────────────
    // Private State
    // ─────────────────────────────────────────────

    private Animator animator;
    private NavMeshAgent agent;

    private enum PedestrianState { Wandering, WaitingToCross, Crossing, Dead }
    private PedestrianState state = PedestrianState.Wandering;

    private bool isDead = false;
    private float stuckTimer = 0f;
    private Vector3 lastPosition;

    // Track which side the pedestrian is currently on
    private bool onSidewalkSide = true;
    private int currentWaypointIndex = 0;

    // ─────────────────────────────────────────────
    // MonoBehaviour
    // ─────────────────────────────────────────────

    void Start()
    {
        agent    = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.speed = Random.Range(minSpeed, maxSpeed);

        animator.ResetTrigger("Die");

        // Start at a random waypoint index
        currentWaypointIndex = Random.Range(0, GetCurrentWaypoints().Length);

        StartCoroutine(WaitForNavMeshThenStart());
    }

    void Update()
    {
        if (isDead) return;

        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 2f)
            {
                stuckTimer = 0f;
                Vector3 newPoint = GetRandomNavMeshPoint(transform.position, wanderRadius);
                agent.SetDestination(newPoint);
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;

        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    // ─────────────────────────────────────────────
    // Helpers — current side waypoints
    // ─────────────────────────────────────────────

    Transform[] GetCurrentWaypoints()
    {
        return onSidewalkSide ? sidewalkWaypoints : otherSideWaypoints;
    }

    Transform[] GetTargetWaypoints()
    {
        return onSidewalkSide ? otherSideWaypoints : sidewalkWaypoints;
    }

    // Get closest waypoint on the current side to cross FROM
    Transform GetCrossStartWaypoint()
    {
        Transform[] waypoints = GetCurrentWaypoints();
        if (waypoints == null || waypoints.Length == 0) return null;

        Transform closest = null;
        float minDist = float.MaxValue;
        foreach (Transform wp in waypoints)
        {
            if (wp == null) continue;
            float dist = Vector3.Distance(transform.position, wp.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = wp;
            }
        }
        return closest;
    }

    // Get the matching waypoint on the other side (same index)
    Transform GetCrossEndWaypoint()
    {
        Transform[] waypoints = GetCurrentWaypoints();
        Transform[] targets   = GetTargetWaypoints();
        if (waypoints == null || targets == null) return null;

        Transform closest = GetCrossStartWaypoint();
        if (closest == null) return null;

        int idx = System.Array.IndexOf(waypoints, closest);
        idx = Mathf.Clamp(idx, 0, targets.Length - 1);
        return targets[idx];
    }

    // ─────────────────────────────────────────────
    // STAGE 0 — Wait for NavMesh placement
    // ─────────────────────────────────────────────

    IEnumerator WaitForNavMeshThenStart()
    {
        yield return new WaitUntil(() => agent.isOnNavMesh);
        StartCoroutine(WanderRoutine());
    }

    // ─────────────────────────────────────────────
    // STAGE 1 — Wander along sidewalk waypoints
    // ─────────────────────────────────────────────

    IEnumerator WanderRoutine()
    {
        while (!isDead && state == PedestrianState.Wandering)
        {
            Transform[] waypoints = GetCurrentWaypoints();

            Vector3 destination;

            if (waypoints != null && waypoints.Length > 0)
            {
                // Walk to next waypoint in sequence
                Transform wp = waypoints[currentWaypointIndex];
                destination  = wp != null ? wp.position : transform.position;

                // Advance to next waypoint (ping-pong or loop)
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                // Fallback: random wander if no waypoints assigned
                destination = GetRandomNavMeshPoint(transform.position, wanderRadius);
            }

            agent.SetDestination(destination);

            yield return new WaitUntil(() =>
                !isDead &&
                agent.isOnNavMesh &&
                !agent.pathPending &&
                agent.remainingDistance <= agent.stoppingDistance + 0.5f
            );

            if (isDead) yield break;

            float pause = Random.Range(minWanderWait, maxWanderWait);
            yield return new WaitForSeconds(pause);

            if (isDead) yield break;

            // Decide whether to cross
            bool decideToCross = Random.value < crossChance;
            Transform crossStart = GetCrossStartWaypoint();
            Transform crossEnd   = GetCrossEndWaypoint();

            if (decideToCross && crossStart != null && crossEnd != null)
            {
                state = PedestrianState.WaitingToCross;
                StartCoroutine(CrossingRoutine(crossStart, crossEnd));
                yield break;
            }
        }
    }

    // ─────────────────────────────────────────────
    // STAGE 2 — Walk to curb, obey light, cross
    // ─────────────────────────────────────────────

    IEnumerator CrossingRoutine(Transform crossStart, Transform crossEnd)
    {
        // Walk to the curb on our side
        agent.SetDestination(crossStart.position);

        yield return new WaitUntil(() =>
            !isDead &&
            agent.isOnNavMesh &&
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f
        );

        if (isDead) yield break;

        // ── Traffic light check ────────────────────
        if (!isRuleBreaker)
        {
            if (trafficLight != null)
            {
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);

                while (!isDead && !trafficLight.IsGreen)
                    yield return null;

                agent.isStopped = false;
            }
            else
            {
                float waitTime = Random.Range(minWaitBeforeCross, maxWaitBeforeCross);
                yield return new WaitForSeconds(waitTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.3f));
        }

        if (isDead) yield break;

        // ── Cross the road ─────────────────────────
        state = PedestrianState.Crossing;
        agent.SetDestination(crossEnd.position);

        yield return new WaitUntil(() =>
            !isDead &&
            agent.isOnNavMesh &&
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f
        );

        if (isDead) yield break;

        // Swap to the other side
        onSidewalkSide = !onSidewalkSide;
        currentWaypointIndex = 0;
        state = PedestrianState.Wandering;

        StartCoroutine(WanderRoutine());
    }

    // ─────────────────────────────────────────────
    // STAGE 3 — Collision detection
    // ─────────────────────────────────────────────

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(truckTag))
            HandleHitByTruck();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(truckTag))
            HandleHitByTruck();
    }

    void HandleHitByTruck()
    {
        if (isDead) return;

        isDead = true;
        state  = PedestrianState.Dead;

        agent.isStopped = true;
        agent.enabled   = false;

        Debug.Log(gameObject.name + " hit by truck — Game Over!");

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        if (animator != null)
            animator.SetTrigger(deathAnimTrigger);

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SinkIntoGround());

        Destroy(gameObject);
    }

    private IEnumerator SinkIntoGround()
    {
        float duration = 1.2f;
        float elapsed  = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = new Vector3(
                originalScale.x,
                Mathf.Lerp(originalScale.y, 0f, t),
                originalScale.z
            );

            yield return null;
        }
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    Vector3 GetRandomNavMeshPoint(Vector3 origin, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir  = Random.insideUnitSphere * radius;
            randomDir         += origin;
            randomDir.y        = origin.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, radius, NavMesh.AllAreas))
                if (Vector3.Distance(origin, hit.position) > 3f)
                    return hit.position;
        }
        return origin;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
    //     Gizmos.DrawSphere(transform.position, wanderRadius);

    //     // Draw sidewalk waypoints in yellow
    //     if (sidewalkWaypoints != null)
    //     {
    //         Gizmos.color = Color.yellow;
    //         for (int i = 0; i < sidewalkWaypoints.Length; i++)
    //         {
    //             if (sidewalkWaypoints[i] == null) continue;
    //             Gizmos.DrawSphere(sidewalkWaypoints[i].position, 0.3f);
    //             if (i + 1 < sidewalkWaypoints.Length && sidewalkWaypoints[i + 1] != null)
    //                 Gizmos.DrawLine(sidewalkWaypoints[i].position, sidewalkWaypoints[i + 1].position);
    //         }
    //     }

    //     // Draw other side waypoints in cyan
    //     if (otherSideWaypoints != null)
    //     {
    //         Gizmos.color = Color.cyan;
    //         for (int i = 0; i < otherSideWaypoints.Length; i++)
    //         {
    //             if (otherSideWaypoints[i] == null) continue;
    //             Gizmos.DrawSphere(otherSideWaypoints[i].position, 0.3f);
    //             if (i + 1 < otherSideWaypoints.Length && otherSideWaypoints[i + 1] != null)
    //                 Gizmos.DrawLine(otherSideWaypoints[i].position, otherSideWaypoints[i + 1].position);
    //         }
    //     }

    //     // Draw crossing lines between matching waypoints
    //     if (sidewalkWaypoints != null && otherSideWaypoints != null)
    //     {
    //         Gizmos.color = Color.red;
    //         int count = Mathf.Min(sidewalkWaypoints.Length, otherSideWaypoints.Length);
    //         for (int i = 0; i < count; i++)
    //         {
    //             if (sidewalkWaypoints[i] != null && otherSideWaypoints[i] != null)
    //                 Gizmos.DrawLine(sidewalkWaypoints[i].position, otherSideWaypoints[i].position);
    //         }
    //     }
    // }
}