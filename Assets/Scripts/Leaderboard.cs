using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> names;
    [SerializeField] List<TextMeshProUGUI> scores;
    [SerializeField] GameObject leaderboardCanvas;
    [SerializeField] GameObject BackGroundImage;
    [SerializeField] TMP_InputField playerName;
    [SerializeField] GameObject submit;
    [SerializeField] GameObject scorePanel;

    private string leaderboardID = "Truck_Safety_Game_LeaderBoard";

    //populate/update leaderboard
    public async void GetLeaderboard()
    {
        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
        int reps = Mathf.Min(leaderboardScoresPage.Results.Count, names.Count);
        for(int i = 0; i < reps; i++)
        {
            names[i].text = leaderboardScoresPage.Results[i].PlayerName.Substring(0, 3);
            scores[i].text = leaderboardScoresPage.Results[i].Score.ToString("000000");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {//anonymous sign-in (no login needed)
        await UnityServices.InitializeAsync();
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch{}
        //Disable leaderboard on start
        leaderboardCanvas.SetActive(false);
        submit.SetActive(false);
        BackGroundImage.SetActive(false);
    }

    //show leaderboard after gameover
    public void show()
    {
        //fill in the leaderboard
        GetLeaderboard();

        //hide the game over UI
        BackGroundImage.SetActive(false);

        //show leaderboard UI
        leaderboardCanvas.SetActive(true);

        //show player score
        scorePanel.SetActive(true);

        //score check

        CheckScore();
    }

    //check to see if player can add name to leaderboard top 10
    private async void CheckScore()
    {
        float score = ScoreHandler.instance.GetScore();

        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);

        if(leaderboardScoresPage.Results.Count < 10)
        {
            submit.SetActive(true);
            return;
        }

        for (int i = 0; i < names.Count; i++)
        {
            if(score > leaderboardScoresPage.Results[i].Score)
            {
                submit.SetActive(true);
                break;
            }
        }

    }

//add user's score if the score is better than the top 10 or if there's less than 10 players in the leaderboard
    public async void AddScore()
    {
        float score = ScoreHandler.instance.GetScore();
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score);
        if(playerName.text != null)
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName.text);
        }

        GetLeaderboard();

    } 
    
    //resume game
    public void Back()
    {
        leaderboardCanvas.SetActive(false);
        submit.SetActive(false);

        BackGroundImage.SetActive(true);
    }
}
