using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideButton : MonoBehaviour
{
    [SerializeField] GameObject guideButton;

    public void show()
    {
        SceneManager.LoadScene("Guide", LoadSceneMode.Additive);
    }
}
