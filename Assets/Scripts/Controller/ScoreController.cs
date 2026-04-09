using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private static ScoreController instance;
    public static ScoreController Instance => instance;

    private ScoreModel scoreModel;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        scoreModel = new ScoreModel();
    }

    public int GetBestScore() => scoreModel.BestScore;
    public List<ScoreEntry> GetScoreHistory() => scoreModel.ScoreHistory;
    public void AddScore(int score) => scoreModel.AddScore(score);
    public void DeleteAllScores() => scoreModel.DeleteScores();
}
