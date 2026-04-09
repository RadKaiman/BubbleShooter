using UnityEngine;
using UnityEngine.UI;

public class AboutView : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button socialButton;
    [SerializeField] private string socialUrl = "https://t.me/RadKaiman";

    private SceneController sceneController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneController = SceneController.Instance;

        backButton.onClick.AddListener(() => sceneController?.LoadMainMenu());

        socialButton.onClick.AddListener(() => Application.OpenURL(socialUrl));
    }
}
