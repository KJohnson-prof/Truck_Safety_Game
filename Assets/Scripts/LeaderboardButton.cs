using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardButton : MonoBehaviour
{
    public void show()
    {
        Debug.Log("start");
        SceneManager.LoadScene("Leaderboard", LoadSceneMode.Additive);
        Debug.Log("end");
    }
}
