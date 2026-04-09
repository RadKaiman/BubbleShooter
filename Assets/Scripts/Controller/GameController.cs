using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameView view;
    [SerializeField] private BallController ballController;
    [SerializeField] private MatchController matchController;
    [SerializeField] private PhysicsController physicsController;

    private GameModel model;
    private List<BallModel> activeBalls = new List<BallModel>();
    private int nextBallColor;
    private bool isGameOver;
    private int initialTopRowCount;

    public GameState CurrentState => model.State;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await StartNewGame();
    }

    void Awake()
    {
        model = new GameModel();
    }

    public async UniTask StartNewGame()
    {
        isGameOver = false;
        model.State = GameState.Waiting;
        view.HidePanels();

        var level = new LevelModel("level1");
        initialTopRowCount = level.InitialTopRowCount;
        activeBalls = level.GenerateBalls();
        model.RemainingShots = level.StartBallCount;
        model.CurrentScore = 0;

        await ballController.InitializeField(activeBalls);

        nextBallColor = Random.Range(0, 3);
        SpawnNextBall();

        model.State = GameState.Aiming;
        UpdateUI();
    }

    public void SpawnNextBall()
    {
        if (isGameOver) return;

        int color = nextBallColor;

        nextBallColor = Random.Range(0, 3);

        ballController.SpawnShootBall(color);

        UpdateUI();
    }

    public void SetState(GameState newState)
    {
        if (!isGameOver || newState == GameState.Aiming) model.State = newState;
    }

    public async UniTask ProcessShotAfterPlacement(BallModel placedBall)
    {
        if (model.State != GameState.Shooting || isGameOver) return;

        model.UseShot();
        UpdateUI();

        activeBalls = ballController.GetAllActiveBalls();

        var matchResult = await matchController.ProcessMatches(placedBall, activeBalls);
        model.AddScore(matchResult.points);
        activeBalls = ballController.GetAllActiveBalls();

        await matchController.ProcessHangingBalls(activeBalls);
        activeBalls = ballController.GetAllActiveBalls();

        UpdateUI();
        await CheckGameConditions();

        if (model.IsOutOfShots() && !isGameOver) await LoseGame();
        UpdateUI();
    }

    private async UniTask CheckGameConditions()
    {
        if (isGameOver) return;

        bool isWin = await matchController.CheckWinCondition(activeBalls, initialTopRowCount);

        if (isWin)
        {
            await WinGame();
        }
    }

    private async UniTask WinGame()
    {
        isGameOver = true;
        model.State = GameState.Victory;
        ScoreController.Instance.AddScore(model.CurrentScore);

        await ballController.FallAllBalls(activeBalls);
        activeBalls.Clear();

        await UniTask.Delay(2000);

        view.ShowWin();

        await UniTask.Delay(2000);

        SceneController.Instance.LoadMainMenu();
    }

    private async UniTask LoseGame()
    {
        isGameOver = true;
        model.State = GameState.GameOver;
        view.ShowLose();
        await UniTask.Delay(2000);
        SceneController.Instance.LoadMainMenu();
    }

    private void UpdateUI()
    {
        view.UpdateUI(model, nextBallColor);
    }
}
