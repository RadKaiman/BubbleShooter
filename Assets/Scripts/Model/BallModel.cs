using UnityEngine;

public class BallModel
{
    public int ColorId { get; set; }
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public bool IsAttachedToCeiling { get; set; }

    public BallModel(int colorId, Vector2 position)
    {
        ColorId = colorId;
        Position = position;
        IsActive = true;
        IsAttachedToCeiling = false;
    }

    public bool IsSameColor(BallModel other) => other != null && ColorId == other.ColorId;
}
