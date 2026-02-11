using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TextMeshProUGUI[] scoreTexts;
    public Button restartButton;

    private void Start()
    {
        HighScoreManager.Load();

        // Fill UI
        for (int i = 0; i < scoreTexts.Length; i++)
            scoreTexts[i].text = $"{i + 1}. {HighScoreManager.Names[i]} - {HighScoreManager.Scores[i]}";

        // Show input only if needed
        int finalScore = GameManager.instance.score;
        bool qualifies = finalScore > HighScoreManager.Scores[4];

        nameInput.gameObject.SetActive(qualifies);

        if (qualifies)
        {
            nameInput.text = "";
            nameInput.onEndEdit.AddListener(SubmitScore);
        }

        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("StartScreen");
        });
    }

    private void SubmitScore(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "PLAYER";

        HighScoreManager.TryInsert(playerName, GameManager.instance.score);
        HighScoreManager.Save();

        // Refresh UI
        for (int i = 0; i < scoreTexts.Length; i++)
            scoreTexts[i].text = $"{i + 1}. {HighScoreManager.Names[i]} - {HighScoreManager.Scores[i]}";

        nameInput.gameObject.SetActive(false);
    }
}
