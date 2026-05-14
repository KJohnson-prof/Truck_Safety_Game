using UnityEngine;

public class BuildingCollision : MonoBehaviour
{
    public string truckTag = "Truck";

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Property"))
        {
            if (GameOverUI.Instance != null)
                GameOverUI.Instance.ShowGameOver(0f, "You hit a barrier!",  "", 
                    "This usually happens from taking a turn too fast, drifting out of your lane, " +
                    "or not adjusting your steering in time. In a real truck, momentum is harder to " +
                    "correct — small mistakes turn into big ones quickly.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(truckTag))
            HandleHitByTruck();
    }

    void HandleHitByTruck()
    {
        Debug.Log("GAME OVER - Truck hit a property!");
        Object.FindFirstObjectByType<TruckController>()?.StopEngineSound();
        float score = Object.FindFirstObjectByType<ScoreHandler>().GetScore();

        if (GameOverUI.Instance != null)
            GameOverUI.Instance.ShowGameOver(score, "You hit a barrier!", "",
                "This usually happens from taking a turn too fast, drifting out of your lane, " +
                "or not adjusting your steering in time. In a real truck, momentum is harder to " +
                "correct — small mistakes turn into big ones quickly.");
    }
}