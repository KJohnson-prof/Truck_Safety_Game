using HealthbarGames;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WarningTrigger : MonoBehaviour
{
    //get traffic light
    [SerializeField] GameObject trafficLight;

    //get warning_canvas
    [SerializeField] GameObject warningCanvas;
    Coroutine showCoroutine;
    static bool gameOverTriggered = false;

    // Reset static variables everytime after user restarts the game to prevent carryover warnings count
    void Start()
    {
        WarningManager.ResetTTotalWarnings();
        gameOverTriggered = false;
    }
        
    private void OnTriggerEnter(Collider other)
    {
        // stop processing after game over
        if (gameOverTriggered) return;

        Light redLight = trafficLight.GetComponent<Light>();

        //if traffic light is red while truck is passing they ran a red light
        if (other.CompareTag("Truck") && redLight.enabled)
        {
            WarningManager.IncreaseTotalWarnings();
            Debug.Log("Warnings: " + WarningManager.GetTotalWarnings());

            // restart warning banner (don't stack)
            if (showCoroutine != null)
                StopCoroutine(showCoroutine);

            showCoroutine = StartCoroutine(show());

            // check game over
            if (WarningManager.GetTotalWarnings() >= 3)
            {
                Debug.Log("GAME OVER - You ran 3 red lights!");
                gameOverTriggered = true;

                float currentScore = ScoreHandler.instance.GetScore();

                // Show game over UI with score
                if (GameOverUI.Instance != null)
                {
                    GameOverUI.Instance.ShowGameOver(
                        score: currentScore,
                        cause: "You ran 3 red lights!",
                        zone: "",
                        detail: "Whether it was rolling through a stop, misjudging the timing, or pushing through a yellow " +
                                "that turned — each one is a chance for a side-impact crash. On the third strike, the run ends. " +
                                "Red means full stop, every time."
                    );
                }            
            }
        }
    }

    
   private IEnumerator show()
    {
        warningCanvas.SetActive(true);
        yield return new WaitForSeconds(3);
        warningCanvas.SetActive(false);
        showCoroutine = null;
    }
}
