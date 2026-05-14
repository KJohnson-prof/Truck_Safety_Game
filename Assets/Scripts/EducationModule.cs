using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EducationModule : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("The education slideshow panel -  active at start")]
    public GameObject educationPanel;

    [Tooltip("Your existing Main Menu panel - inactive at start")]
    public GameObject mainMenuPanel;

    [Header("Slides")]
    [Tooltip("Assign Slide_1 through Slide_4 GameObjects here in order")]
    public GameObject[] slides;

    [Header("Navigation Buttons")]
    public Button prevButton;
    public Button nextButton;
    public Button skipButton;   // always visible, jumps straight to main menu
    public Button playButton;   // only shown on last slide

    [Header("Slide Counter (optional)")]
    [Tooltip("A Text component showing '1 / 4' etc.")]
    public Text slideCounterText;

    private int currentSlide = 0;

    void Start()
    {
        if (slides == null || slides.Length == 0)
        {
            Debug.LogError("EducationModuleManager: No slides assigned!");
            return;
        }

        // Make sure education is shown and main menu is hidden at start
        if (educationPanel) educationPanel.SetActive(true);
        if (mainMenuPanel)  mainMenuPanel.SetActive(false);

        // Wire buttons
        if (prevButton) prevButton.onClick.AddListener(GoToPrevSlide);
        if (nextButton) nextButton.onClick.AddListener(GoToNextSlide);
        if (skipButton) skipButton.onClick.AddListener(FinishModule);
        if (playButton) playButton.onClick.AddListener(FinishModule);

        // Show first slide
        ShowSlide(0);
    }

    public void GoToNextSlide()
    {
        if (currentSlide < slides.Length - 1)
            ShowSlide(currentSlide + 1);
    }

    public void GoToPrevSlide()
    {
        if (currentSlide > 0)
            ShowSlide(currentSlide - 1);
    }

    public void FinishModule()
    {
        if (educationPanel) educationPanel.SetActive(false);
        if (mainMenuPanel)  mainMenuPanel.SetActive(true);
    }
    
    private void ShowSlide(int index)
    {
        // Hide all, show only current
        for (int i = 0; i < slides.Length; i++)
            slides[i].SetActive(i == index);

        currentSlide = index;

        // Slide counter text
        if (slideCounterText != null)
            slideCounterText.text = $"{currentSlide + 1} / {slides.Length}";

        // Prev: hidden on slide 0
        if (prevButton)
            prevButton.gameObject.SetActive(currentSlide > 0);

        // Next: hidden on last slide
        if (nextButton)
            nextButton.gameObject.SetActive(currentSlide < slides.Length - 1);

        // Play button: only on last slide
        if (playButton)
            playButton.gameObject.SetActive(currentSlide == slides.Length - 1);

        // Skip: always visible so player is never trapped
        if (skipButton)
            skipButton.gameObject.SetActive(true);
    }
}