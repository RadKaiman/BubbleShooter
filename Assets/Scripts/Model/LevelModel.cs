using UnityEngine;
using System.Collections.Generic;

public class LevelModel
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int StartBallCount { get; private set; }
    public int[,] GridData { get; private set; }
    public int InitialTopRowCount { get; private set; }



    public LevelModel(string levelName)
    {
        LoadLevel(levelName);
    }

    private void LoadLevel(string levelName)
    {
        TextAsset file = Resources.Load<TextAsset>($"Levels/{levelName}");
        if (file == null)
        {
            Debug.LogError($"Уровня {levelName} нет");
            return;
        }

        string[] lines = file.text.Split('\n');
        string[] dims = lines[0].Trim().Split(',');

        Width = int.Parse(dims[0]);
        Height = int.Parse(dims[1]);
        StartBallCount = int.Parse(dims[2]);

        GridData = new int[Width, Height];

        for (int y = 0; y < Height && y + 1 < lines.Length; y++)
        {
            string[] colors = lines[y + 1].Trim().Split(',');
            for (int x = 0; x < Width && x < colors.Length; x++)
            {
                GridData[x, y] = int.Parse(colors[x]);
            }
        }

        int topRowY = Height - 1;
        int count = 0;
        for (int x = 0; x < Width; x++)
            if (GridData[x, topRowY] >= 0) count++;
        InitialTopRowCount = count;
    }

    public List<BallModel> GenerateBalls()
    {
        List<BallModel> balls = new List<BallModel>();
        float startX = -2.5f;
        float startY = 2.5f;
        float step = 1.05f;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (GridData[x, y] >= 0)
                {
                    Vector2 pos = new Vector2(startX + x * step, startY + y * step);
                    balls.Add(new BallModel(GridData[x, y], pos));
                }
            }
        }
        return balls;
    }
}
