using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    [SerializeField] private float minNormalSpeed = 6f;
    [SerializeField] private float maxNormalSpeed = 14f;
    [SerializeField] private float perfectSpeedMultiplier = 1.6f;

    private Vector2 gravity = new Vector2(0, -15f);
    private float timeStep = 0.02f;
    private float leftBound = -3.5f;
    private float rightBound = 3.5f;
    private float ballRadius = 0.5f;

    private float topBound = 5.1f;

    [SerializeField] private BallController ballController;

    public TrajectoryResult CalculateTrajectory(Vector2 startPos, Vector2 direction, float power, bool isPerfect, List<BallModel> activeBalls)
    {
        TrajectoryResult result = new TrajectoryResult();
        result.points = new List<Vector2>();
        result.hitPoint = Vector2.zero;
        result.hasHit = false;

        float normalSpeed = Mathf.Lerp(minNormalSpeed, maxNormalSpeed, power);
        float speed = isPerfect ? normalSpeed * perfectSpeedMultiplier : normalSpeed;
        Vector2 velocity = direction * speed;
        Vector2 pos = startPos;

        float leftWall = leftBound + ballRadius;
        float rightWall = rightBound - ballRadius;

        for (int i = 0; i < 300; i++)
        {
            result.points.Add(pos);
            velocity += gravity * timeStep;
            Vector2 nextPos = pos + velocity * timeStep;

            if (nextPos.x < leftWall)
            {
                nextPos.x = leftWall + (leftWall - nextPos.x);
                velocity.x = -velocity.x;
            }
            else if (nextPos.x > rightWall)
            {
                nextPos.x = rightWall - (nextPos.x - rightWall);
                velocity.x = -velocity.x;
            }

            if (nextPos.y + ballRadius >= topBound)
            {
                result.hasHit = true;
                result.isCeilingHit = true;
                result.hitPoint = new Vector2(nextPos.x, topBound - ballRadius);
                return result;
            }

            if (nextPos.y < -5f)
                break;

            foreach (var ball in activeBalls)
            {
                if (!ball.IsActive) continue;
                Vector2 toCenter = nextPos - ball.Position;
                float distance = toCenter.magnitude;
                if (distance < ballRadius)
                {
                    Vector2 directionFromCenter = toCenter.normalized;
                    Vector2 surfacePoint = ball.Position + directionFromCenter * ballRadius;
                    result.hasHit = true;
                    result.hitPoint = surfacePoint;
                    return result;
                }
            }
            pos = nextPos;
        }
        return result;
    }

    public Vector2 CalculateDirection(Vector2 startDrag, Vector2 currentDrag)
    {
        return (startDrag - currentDrag).normalized;
    }

    public float CalculatePower(float dragDistance, float maxDistance)
    {
        return Mathf.Clamp01(dragDistance / maxDistance);
    }

    public bool IsPerfectShot(float dragDistance, float maxDistance)
    {
        return dragDistance > maxDistance * 0.95f;
    }

    public void ApplyPushEffect(BallModel hitBall, Vector2 impactPoint, float force)
    {
        ballController.PushBall(hitBall, impactPoint, force);
    }
}

public struct TrajectoryResult
{
    public List<Vector2> points;
    public Vector2 hitPoint;
    public bool hasHit;
    public bool isCeilingHit;
}
