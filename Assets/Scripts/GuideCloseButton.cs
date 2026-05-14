using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviour
{
    //scene will be loaded additively so will return to original scene
    public void close()
    {
        SceneManager.UnloadScene("Guide");
    }
}
