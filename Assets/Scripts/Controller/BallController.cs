using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private GameObject ballPrefab;

    private BallModel currentShootBall;
    private Dictionary<BallModel, BallView> ballViews = new Dictionary<BallModel, BallView>();
    private List<BallModel> allBalls = new List<BallModel>();
    private ObjectPool<BallView> ballPool;

    void Awake()
    {
        var viewPrefab = ballPrefab.GetComponent<BallView>();
        ballPool = new ObjectPool<BallView>(viewPrefab, 50, transform);
    }

    public async UniTask InitializeField(List<BallModel> balls)
    {
        ClearAllBalls();

        foreach (var ball in balls)
        {
            var view = ballPool.Get();
            view.Initialize(ball);
            ballViews[ball] = view;
            allBalls.Add(ball);
            await UniTask.NextFrame();
        }
    }

    public void SpawnShootBall(int colorId)
    {
        if (currentShootBall != null && ballViews.ContainsKey(currentShootBall))
        {
            ReturnBallToPool(currentShootBall);
        }

        currentShootBall = new BallModel(colorId, ballSpawnPoint.position);

        var view = ballPool.Get();
        view.Initialize(currentShootBall);
        ballViews[currentShootBall] = view;
    }

    public BallModel GetCurrentBall()
    {
        return currentShootBall;
    }

    public BallView GetBallView(BallModel ball)
    {
        return ballViews.TryGetValue(ball, out var view) ? view : null;
    }

    public List<BallModel> GetAllActiveBalls()
    {
        return allBalls.Where(b => b.IsActive).ToList();
    }

    public async UniTask UpdateBallPosition(BallModel ball)
    {
        if (ballViews.TryGetValue(ball, out var view))
        {
            await view.transform.DOMove(new Vector3(ball.Position.x, ball.Position.y, 0), 0.1f).ToUniTask();
        }
    }

    public async UniTask MoveBallAlongPath(BallModel ball, List<Vector2> pathPoints)
    {
        if (!ballViews.ContainsKey(ball)) return;

        var view = ballViews[ball];

        for (int i = 0; i < pathPoints.Count; i++)
        {
            await view.transform.DOMove(new Vector3(pathPoints[i].x, pathPoints[i].y, 0), 0.02f)
                .SetEase(Ease.Linear)
                .ToUniTask();

            ball.Position = pathPoints[i];
        }
    }

    public async UniTask ExplodeBall(BallModel ball)
    {
        if (ballViews.TryGetValue(ball, out var view))
        {
            await view.PlayExplosionAsync();
            ReturnBallToPool(ball);
        }
        ball.IsActive = false;
    }

    public void AddBall(BallModel ball)
    {
        if (!allBalls.Contains(ball))
            allBalls.Add(ball);
        ball.IsActive = true;
        if (currentShootBall == ball)
            currentShootBall = null;
    }

    public void RemoveBall(BallModel ball)
    {
            allBalls.Remove(ball);
            ball.IsActive = false;
    }

    public BallModel FindNearestBall(Vector2 point, List<BallModel> balls)
    {
        BallModel nearest = null;
        float minDist = 0.6f;

        foreach (var ball in balls)
        {
            if (!ball.IsActive) continue;
            float dist = Vector2.Distance(point, ball.Position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = ball;
            }
        }
        return nearest;
    }

    public void PushBall(BallModel ball, Vector2 source, float force)
    {
        if (ballViews.TryGetValue(ball, out var view))
        {
            view.PushFrom(source, force);
        }
    }

    public async UniTask FallAllBalls(List<BallModel> balls)
    {
        foreach (var ball in balls)
        {
            if (ballViews.TryGetValue(ball, out var view))
            {
                await view.transform.DOMoveY(-5f, 1f).SetEase(Ease.InQuad).ToUniTask();
                ReturnBallToPool(ball);
            }
        }
        allBalls.Clear();
        if (currentShootBall != null)
            ReturnBallToPool(currentShootBall);
        currentShootBall = null; 
    }

    public void ClearAllBalls()
    {
        foreach (var ball in ballViews)
        {
            ballPool.Return(ball.Value);
        }
        ballViews.Clear();
        allBalls.Clear();

        if (currentShootBall != null)
            currentShootBall = null;
    }

    private void ReturnBallToPool(BallModel ball)
    {
        if (ballViews.TryGetValue(ball, out var view))
        {
            ballPool.Return(view);
            ballViews.Remove(ball);
        }
    }
}
