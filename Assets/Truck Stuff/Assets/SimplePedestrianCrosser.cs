using System.Collections;
using UnityEngine;

public class SimplePedestrianCrosser : MonoBehaviour
{
    [Header("Crossing Settings")]
    public Vector3 crossDirection = Vector3.forward;
    public float crossDistance = 20f;
    public float minSpeed = 1f;
    public float maxSpeed = 1.8f;

    [Header("Truck Collision")]
    public string truckTag = "Truck";

    [Header("Death Animation")]
    public string deathAnimTrigger = "Die";
    public float deathAnimDuration = 2f;

    private Animator animator;
    private bool isDead = false;
    private Vector3 startPosition;
    private float currentSpeed;
    private Vector3 moveDirection;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        startPosition = transform.position;
        moveDirection = crossDirection.normalized;
        BlindSpotDetector.lastHitZone = "";

        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDirection);

        if (animator != null)
            animator.ResetTrigger("Die");
    }
    void Update()
    {
        if (isDead) return;

        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        if (animator != null)
            animator.SetFloat("Speed", currentSpeed);

        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= crossDistance)
        {
            DespawnPedestrian();
        }
    }

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
        Object.FindFirstObjectByType<TruckController>()?.StopEngineSound();

        float score = 0f;
        ScoreHandler scoreHandler = Object.FindFirstObjectByType<ScoreHandler>();
        if (scoreHandler != null)
            score = scoreHandler.GetScore();

        StartCoroutine(DeathSequence());
        StartCoroutine(ShowGameOverNextFrame(score));
    }

    IEnumerator ShowGameOverNextFrame(float score)
    {
        yield return null;
        yield return null;

        string zone = BlindSpotDetector.lastHitZone;

        // only show detail blurb when it's NOT a blind spot hit
        string detail = zone != "" ? "" :
            "This often comes down to speeding through an intersection, missing a mirror check, " +
            "or not accounting for the blind spots around the cab. Real trucks have huge blind zones " +
            "where a person can disappear completely — a quick scan can be the difference between " +
            "a safe turn and a fatal one.";

        if (GameOverUI.Instance != null)
            GameOverUI.Instance.ShowGameOver(score, "You hit a pedestrian!", zone, detail);
    }
    IEnumerator DeathSequence()
    {
        if (animator != null)
            animator.SetTrigger(deathAnimTrigger);

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(SinkIntoGround());

        Destroy(gameObject);
    }

    private IEnumerator SinkIntoGround()
    {
        float duration = 1.2f;
        float elapsed = 0f;
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

    void DespawnPedestrian()
    {
        Debug.Log("Pedestrian reached end of crossing path. Despawning.");
        Destroy(gameObject);
    }
}