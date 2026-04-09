using UnityEngine;
using System;

[Serializable]
public class GameModel
{
    public int CurrentScore { get; set; }
    public int RemainingShots { get; set; }
    public GameState State { get; set; }
    public int CurrentLevel { get; set; }

    public GameModel()
    {
        CurrentScore = 0;
        RemainingShots = 30;
        State = GameState.Menu;
        CurrentLevel = 1;
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
    }

    public void UseShot()
    {
        if (RemainingShots > 0)
            RemainingShots--;
    }

    public bool IsOutOfShots() => RemainingShots <= 0;
}

public enum GameState
{
    Menu,
    Aiming,
    Shooting,
    Waiting,
    GameOver,
    Victory
}
