
using HYPLAY.Core.Runtime;
using HYPLAY.Leaderboards.Runtime;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public HyplayLeaderboard leaderboard;
    public ScoreContainer[] containers;
    public TMPro.TextMeshProUGUI mijnScore;

    public void Awake()
    {
        HyplayBridge.LoggedIn += LoadScores;
        if (HyplayBridge.IsLoggedIn)
            LoadScores();
    }

    float GetScore()
    {
        return PlayerController.main == null ? 0 :  PlayerController.main.Score;
    }
    private void OnEnable()
    {
        if (GetScore()>0) 
        SubmitScore();
    }

    public async void SubmitScore()
    {
        if (mijnScore! != null)
            mijnScore.text = GetScore() + "";
            if (leaderboard == null) return;


        var res = await leaderboard.PostScore(Mathf.RoundToInt(GetScore()));
        if (res.Success)
            Debug.Log($"Successfully posted score {res.Data.score}");
        else
            Debug.LogError($"Update score failed: {res.Error}");

        LoadScores();
    }
    public async void LoadScores()
    {

        foreach (var text in containers)
            text.gameObject.SetActive(false);

        if (leaderboard == null) return;

        var scores = await leaderboard.GetScores( HyplayLeaderboard.OrderBy.descending, 0, containers.Length);
        if (!scores.Success)
        {
            Debug.Log($"Getting scores failed: {scores.Error}");
            return;
        }
        for (var i = 0; i < scores.Data.scores.Length; i++)
        {
            var score = scores.Data.scores[i];
            var text = containers[i];
            text.gameObject.SetActive(true);

            text.player.text = score.username;
            text.score.text = score.score + "";
        }

        var currentUserScore = await leaderboard.GetCurrentUserScore();
        if (!currentUserScore.Success)
        {
            Debug.LogError($"Getting current user score failed: {currentUserScore.Error}");
            return;
        }

        Debug.Log($"Current user score: {currentUserScore.Data}");
    }
}
