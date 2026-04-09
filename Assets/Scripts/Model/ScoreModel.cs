using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScoreModel
{
    private const string BEST_SCORE_KEY = "BestScore";
    private const string SCORE_HISTORY_KEY = "ScoreHistory";
    private const int MAX_HISTORY = 10;

    public int BestScore { get; private set; }
    public List<ScoreEntry> ScoreHistory { get; private set; }

    public ScoreModel()
    {
        LoadScores();
    }

    private void LoadScores()
    {
        BestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        string json = PlayerPrefs.GetString(SCORE_HISTORY_KEY, "");
        if (!string.IsNullOrEmpty(json))
            ScoreHistory = JsonUtility.FromJson<ScoreListWrapper>(json)?.list ?? new List<ScoreEntry>();
        else
            ScoreHistory = new List<ScoreEntry>();
    }

    public void AddScore(int score)
    {
        if (score > BestScore)
        {
            BestScore = score;
            PlayerPrefs.SetInt(BEST_SCORE_KEY, BestScore);
        }

        ScoreHistory.Add(new ScoreEntry(score));
        ScoreHistory = ScoreHistory.OrderByDescending(s => s.score).Take(MAX_HISTORY).ToList();

        var wrapper = new ScoreListWrapper { list = ScoreHistory };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(SCORE_HISTORY_KEY, json);
        PlayerPrefs.Save();
    }

    public void DeleteScores()
    {
        PlayerPrefs.DeleteKey(BEST_SCORE_KEY);
        PlayerPrefs.DeleteKey(SCORE_HISTORY_KEY);
        PlayerPrefs.Save();
        LoadScores();
    }

    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<ScoreEntry> list;
    }
}

[System.Serializable]
public class ScoreEntry
{
    public int score;
    public string date;

    public ScoreEntry(int score)
    {
        this.score = score;
        this.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}