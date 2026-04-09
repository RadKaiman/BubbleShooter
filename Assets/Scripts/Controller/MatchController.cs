using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class MatchController : MonoBehaviour
{
    [SerializeField] private BallController ballController;
    [SerializeField] private float ceilingY = 4.7f;

    public async UniTask<(List<BallModel> matchedBalls, int points)> ProcessMatches(BallModel startBall, List<BallModel> allBalls)
    {
        var matchedGroup = GetConnectedGroup(startBall, allBalls);
        int points = 0;

        if (matchedGroup.Count >= 3)
        {
            points = matchedGroup.Count * 100;

            foreach (var ball in matchedGroup)
            {
                await ballController.ExplodeBall(ball);
                allBalls.Remove(ball);
                await UniTask.Delay(50);
            }
        }

        return (matchedGroup, points);
    }

    public async UniTask ProcessHangingBalls(List<BallModel> allBalls)
    {
        var hangingBalls = FindHangingBalls(allBalls);

        foreach (var ball in hangingBalls)
        {
            await ballController.ExplodeBall(ball);
            allBalls.Remove(ball);
            await UniTask.Delay(50);
        }
    }

    public async UniTask<bool> CheckWinCondition(List<BallModel> allBalls, int initialTopRowCount)
    {
        await UniTask.NextFrame();

        if (allBalls.Count == 0) return true;
        if (initialTopRowCount == 0) return false;

        float maxY = allBalls.Max(b => b.Position.y);
        float threshold = maxY - 0.2f;
        int currentTopBalls = allBalls.Count(b => b.Position.y >= threshold);
        bool win = currentTopBalls < 0.3f * initialTopRowCount;
        return win;
    }

    public List<BallModel> GetConnectedGroup(BallModel startBall, List<BallModel> allBalls)
    {
        Queue<BallModel> queue = new Queue<BallModel>();
        List<BallModel> group = new List<BallModel>();
        HashSet<BallModel> visited = new HashSet<BallModel>();

        queue.Enqueue(startBall);
        visited.Add(startBall);

        while (queue.Count > 0)
        {
            BallModel current = queue.Dequeue();
            group.Add(current);

            foreach (var neighbor in GetNeighbors(current, allBalls))
            {
                if (!visited.Contains(neighbor) && current.IsSameColor(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return group;
    }

    public List<BallModel> FindHangingBalls(List<BallModel> allBalls)
    {
        HashSet<BallModel> attachedToCeiling = new HashSet<BallModel>();
        Queue<BallModel> queue = new Queue<BallModel>();

        foreach (var ball in allBalls)
        {
            if (ball.Position.y >= ceilingY - 0.2f)
            {
                queue.Enqueue(ball);
                attachedToCeiling.Add(ball);
            }
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var neighbor in GetNeighbors(current, allBalls))
            {
                if (!attachedToCeiling.Contains(neighbor))
                {
                    attachedToCeiling.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return allBalls.Where(b => !attachedToCeiling.Contains(b)).ToList();
    }

    private List<BallModel> GetNeighbors(BallModel ball, List<BallModel> allBalls)
    {
        List<BallModel> neighbors = new List<BallModel>();
        float neighborDistance = 1.1f;

        foreach (var other in allBalls)
        {
            if (other == ball || !other.IsActive) continue;
            if (Vector2.Distance(ball.Position, other.Position) < neighborDistance)
            {
                neighbors.Add(other);
            }
        }

        return neighbors;
    }
}
