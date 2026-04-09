using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class GameView : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text shotsText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Image nextBallPreview;
    [SerializeField] private Button toMenuButton;

    private SceneController sceneController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneController = SceneController.Instance;

        toMenuButton.onClick.AddListener(() => sceneController?.LoadMainMenu());
    }

    public void UpdateUI(GameModel model, int nextBallColor)
    {
        scoreText.text = $"Score: {model.CurrentScore}";
        shotsText.text = $"Shots: {model.RemainingShots}";

        nextBallPreview.color = GetColorByID(nextBallColor);
    }

    public async void ShowWin()
    {
        winPanel.SetActive(true);
        winPanel.transform.localScale = Vector3.zero;
        await winPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).ToUniTask();
    }

    public async void ShowLose()
    {
        losePanel.SetActive(true);
        losePanel.transform.localScale = Vector3.zero;
        await losePanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).ToUniTask();
    }

    public void HidePanels()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    private static string GetColorName(int id)
    {
        switch (id)
        {
            case 0: return "Red";
            case 1: return "Blue";
            case 2: return "Green";
            default: return "Unknown";
        }
    }

    private static Color GetColorByID(int id)
    {
        switch (id)
        {
            case 0: return Color.red;
            case 1: return Color.blue;
            case 2: return Color.green;
            default: return Color.white;
        }
    }
}
