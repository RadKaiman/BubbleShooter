using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private Transform historyContainer;
    [SerializeField] private GameObject historyEntryPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (bestScoreText != null)
            bestScoreText.text = $"Best Score: {ScoreController.Instance.GetBestScore()}";

        foreach (Transform child in historyContainer)
            Destroy(child.gameObject);

        foreach (var entry in ScoreController.Instance.GetScoreHistory())
        {
            GameObject entryObj = Instantiate(historyEntryPrefab, historyContainer);
            TMP_Text entryText = entryObj.GetComponent<TMP_Text>();
            if (entryText != null)
                entryText.text = $"{entry.score}   ({entry.date})";
        }
    }
}
