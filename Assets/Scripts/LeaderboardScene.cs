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
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LeaderboardScene : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> names;
    [SerializeField] List<TextMeshProUGUI> scores;

    private string leaderboardID = "Truck_Safety_Game_LeaderBoard";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        //anonymous sign-in (no login needed)
        await UnityServices.InitializeAsync();
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch { }

        GetLeaderboard();
    }

    public async void GetLeaderboard()
    {
        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
        int reps = Mathf.Min(leaderboardScoresPage.Results.Count, names.Count);
        for (int i = 0; i < reps; i++)
        {
            names[i].text = leaderboardScoresPage.Results[i].PlayerName.Substring(0, 3);
            scores[i].text = leaderboardScoresPage.Results[i].Score.ToString("000000");
        }
    }
}
