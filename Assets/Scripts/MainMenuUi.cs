using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialImage;
    [SerializeField] private GameObject safetyGuidePanel;  // Changed from safetyGuideScrollView
    
    public void OnStartButton()
    {
        Debug.Log("🎮 Starting Game...");
        SceneManager.LoadScene(1);
    }

    public void OnTutorialButton()
    {
        Debug.Log("📖 Opening Tutorial...");
        if (tutorialImage != null)
        {
            tutorialImage.SetActive(true);
        }
    }

    public void OnSafetyGuideButton()
    {
        Debug.Log("🚛 Opening Truck Safety Guide...");
        
        SceneManager.LoadScene("Guide", LoadSceneMode.Additive);
    }

    public void OnCloseButton()
    {
        Debug.Log("❌ Closing Tutorial...");
        if (tutorialImage != null)
        {
            tutorialImage.SetActive(false);
        }
    }

    public void OnCloseSafetyGuideButton()
    {
        Debug.Log("❌ Closing Safety Guide...");
        if (safetyGuidePanel != null)  // Changed
        {
            safetyGuidePanel.SetActive(false);  // Changed
        }
    }

    public void QuitGame()
    {
        Debug.Log("❌ Quitting Game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}