using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Highscore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] TextMeshProUGUI scoreText;
    
    public const string TAG_HIGHSCORE = "Highscore";
    public const string TAG_SCORE = "CurrentScore";

    void Start()
    {
        // 
        int highscore = PlayerPrefs.GetInt(TAG_HIGHSCORE, 0);
        highscoreText.SetText("Highscore: " + highscore);

        int currentScore = PlayerPrefs.GetInt(TAG_SCORE, 0);
        scoreText.SetText("Score: " + currentScore);

        PlayerPrefs.DeleteKey(TAG_SCORE);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
