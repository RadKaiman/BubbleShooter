using Cysharp.Threading.Tasks;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private BallController ballController;
    [SerializeField] private PhysicsController physicsController;
    [SerializeField] private TrajectoryView trajectoryView;
    [SerializeField] private GameController gameController;

    [SerializeField] private float maxDragDistance = 200f;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float spreadDistance = 0.2f;

    private Vector2 dragStartScreen;
    private Vector2 dragStartWorld;
    private bool isDragging = false;
    private BallModel currentBallModel;
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.CurrentState != GameState.Aiming)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            currentBallModel = ballController.GetCurrentBall();
            if (currentBallModel != null)
            {
                float distance = Vector2.Distance(mouseWorldPos, ballController.GetBallView(currentBallModel).transform.position);

                if (distance < 0.6f)
                {
                    isDragging = true;
                    dragStartWorld = mouseWorldPos;
                    dragStartScreen = Input.mousePosition;
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && currentBallModel != null)
        {
            Vector2 currentScreen = Input.mousePosition;

            float dragDistance = Vector2.Distance(dragStartScreen, currentScreen);
            float power = physicsController.CalculatePower(dragDistance, maxDragDistance);
            bool isPerfect = physicsController.IsPerfectShot(dragDistance, maxDragDistance);
            Vector2 direction = physicsController.CalculateDirection(dragStartWorld, mainCamera.ScreenToWorldPoint(currentScreen));

            var activeBalls = ballController.GetAllActiveBalls();

            if (isPerfect)
            {
                Vector2 dirLeft = Quaternion.Euler(0, 0, spreadAngle) * direction;
                Vector2 dirRight = Quaternion.Euler(0, 0, -spreadAngle) * direction;
                var trajLeft = physicsController.CalculateTrajectory(ballController.GetBallView(currentBallModel).transform.position, dirLeft, power, true, activeBalls);
                var trajRight = physicsController.CalculateTrajectory(ballController.GetBallView(currentBallModel).transform.position, dirRight, power, true, activeBalls);
                trajectoryView.ShowTrajectory(null, true, trajLeft.points, trajRight.points);
            }
            else
            {
                var traj = physicsController.CalculateTrajectory(ballController.GetBallView(currentBallModel).transform.position, direction, power, false, activeBalls);
                trajectoryView.ShowTrajectory(traj.points, false);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging && currentBallModel != null)
        {
            isDragging = false;

            Vector2 endScreen = Input.mousePosition;
            float dragDistance = Vector2.Distance(dragStartScreen, endScreen);
            float power = physicsController.CalculatePower(dragDistance, maxDragDistance);
            bool isPerfect = physicsController.IsPerfectShot(dragDistance, maxDragDistance);
            Vector2 direction = physicsController.CalculateDirection(dragStartWorld, mainCamera.ScreenToWorldPoint(endScreen));

            trajectoryView.HideTrajectory();

            Shoot(currentBallModel, direction, power, isPerfect).Forget();

            currentBallModel = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = false;
            trajectoryView.HideTrajectory();
            currentBallModel = null;
        }
    }

    private async UniTaskVoid Shoot(BallModel ball, Vector2 direction, float power, bool isPerfect)
    {
        gameController.SetState(GameState.Shooting);
        var startPos = ballController.GetBallView(ball).transform.position;
        var activeBalls = ballController.GetAllActiveBalls();

        Vector2 finalDir = direction;

        if (isPerfect)
        {
            float randomAngle = Random.Range(-spreadAngle, spreadAngle);
            finalDir = Quaternion.Euler(0, 0, randomAngle) * direction;
        }

        var result = physicsController.CalculateTrajectory(startPos, finalDir, power, isPerfect, activeBalls);

        await ballController.MoveBallAlongPath(ball, result.points);

        if (result.hasHit)
        {
            if (result.isCeilingHit)
            {
                ball.Position = result.hitPoint;
                await ballController.UpdateBallPosition(ball);
                ballController.AddBall(ball);
                await gameController.ProcessShotAfterPlacement(ball);
            }
            else
            {
                var hitBall = ballController.FindNearestBall(result.hitPoint, activeBalls);
                if (hitBall != null)
                {
                    if (isPerfect)
                    {
                        await ballController.ExplodeBall(hitBall);
                        ballController.RemoveBall(hitBall);
                        ball.Position = hitBall.Position;
                        await ballController.UpdateBallPosition(ball);
                        ballController.AddBall(ball);
                        await gameController.ProcessShotAfterPlacement(ball);
                    }
                    else
                    {
                        Vector2 normal = (result.hitPoint - hitBall.Position).normalized;
                        Vector2 attachPos = hitBall.Position + normal * 1f;
                        ball.Position = attachPos;
                        await ballController.UpdateBallPosition(ball);
                        ballController.AddBall(ball);
                        physicsController.ApplyPushEffect(hitBall, result.hitPoint, 0.3f);
                        await gameController.ProcessShotAfterPlacement(ball);
                    }
                }
                else
                {
                    ball.Position = result.hitPoint;
                    await ballController.UpdateBallPosition(ball);
                    ballController.AddBall(ball);
                    await gameController.ProcessShotAfterPlacement(ball);
                }
            }
        }
        else
        {
            await ballController.ExplodeBall(ball);
            ballController.RemoveBall(ball);
        }

        gameController.SpawnNextBall();
        gameController.SetState(GameState.Aiming);
    }
}