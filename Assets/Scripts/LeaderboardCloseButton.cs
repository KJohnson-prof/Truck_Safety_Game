using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardCloseButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Back()
    {
        SceneManager.UnloadScene("Leaderboard");
    }
}
