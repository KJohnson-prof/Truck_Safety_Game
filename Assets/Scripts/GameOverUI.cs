using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;
    public GameObject backgroundImage;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI causeText;
    [SerializeField] TextMeshProUGUI gameOverReasonText;
    [SerializeField] TextMeshProUGUI mistakeDetailText;  // ← teaching blurb
    [SerializeField] GameObject scoreUI;

    [Header("Blind Spot Image")]
    [SerializeField] GameObject blindSpotImage;

    void Awake()
    {
        Instance = this;
        backgroundImage.SetActive(false);
        if (blindSpotImage != null)
            blindSpotImage.SetActive(false);
    }

    public void ShowGameOver(float score = 0f, string cause = "", string zone = "", string detail = "")
    {
        backgroundImage.SetActive(true);
        if (scoreUI != null) scoreUI.SetActive(false);
        Time.timeScale = 0f;

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score.ToString("000000");

        if (causeText != null)
            causeText.text = cause;

        // shows for barrier/car/pedestrian, not blind spot
        if (mistakeDetailText != null)
            mistakeDetailText.text = detail;

        // blind spot zone
        if (zone != "")
        {
            if (blindSpotImage != null) blindSpotImage.SetActive(true);
            if (gameOverReasonText != null)
                gameOverReasonText.text = $"Blind spot hit: {zone}";
        }
        else
        {
            if (blindSpotImage != null) blindSpotImage.SetActive(false);
            if (gameOverReasonText != null)
                gameOverReasonText.text = "";
        }
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        BlindSpotDetector.lastHitZone = "";
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMenuButton()
    {
        Time.timeScale = 1f;
        BlindSpotDetector.lastHitZone = "";
        SceneManager.LoadScene("MainMenuScene");
    }
}