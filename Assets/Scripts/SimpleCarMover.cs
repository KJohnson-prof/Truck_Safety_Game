using UnityEngine;

public class SimpleCarMover : MonoBehaviour
{
    [Header("Collision Sound")]
    public AudioClip collisionSound;
    private AudioSource audioSource;

    public Vector3 moveDirection = Vector3.forward;
    public float speed = 8f;
    public float travelDistance = 60f;
    public string truckTag = "Truck";
    public bool isMoving = true;

    private Vector3 startPosition;

    void Start()
    {   
        // initialize the starting position for distance tracking
        startPosition = transform.position;
        if (isMoving && moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized);

        // Audio setting
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.volume = 1f;
    }

    void Update()
    {   
        // To set the car is used for static or moving
        if (!isMoving) return;

        transform.position += moveDirection.normalized * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= travelDistance)
            Destroy(gameObject);
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
        Debug.Log("GAME OVER - Truck collided with a car!");
        Object.FindFirstObjectByType<TruckController>()?.StopEngineSound();

        float score = Object.FindFirstObjectByType<ScoreHandler>().GetScore();

        if (collisionSound != null)
            audioSource.PlayOneShot(collisionSound);

        if (GameOverUI.Instance != null)
            GameOverUI.Instance.ShowGameOver(score, "You hit a vehicle!", "",
                "This usually happens from taking a turn too fast, drifting out of your lane, " +
                "or not adjusting your steering in time. In a real truck, momentum is harder to " +
                "correct — small mistakes turn into big ones quickly.");

        Destroy(gameObject, 0.5f);
    }
}
