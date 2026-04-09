using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private Button confirmExitButton;
    [SerializeField] private Button cancelExitButton;
    [SerializeField] private Button deleteScoreButton;
    [SerializeField] private ScoreView scoreView;

    private SceneController sceneController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneController = SceneController.Instance;

        newGameButton.onClick.AddListener(() => sceneController?.LoadGameplay());
        aboutButton.onClick.AddListener(() => sceneController?.LoadAbout());
        exitButton.onClick.AddListener(() => confirmPanel.SetActive(true));
        confirmExitButton.onClick.AddListener(() => sceneController?.QuitGame());
        cancelExitButton.onClick.AddListener(() => confirmPanel.SetActive(false));
        confirmPanel.SetActive(false);
        deleteScoreButton.onClick.AddListener(() => {
            ScoreController.Instance.DeleteAllScores();
            scoreView.UpdateUI();
        });
    }
}
