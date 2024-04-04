using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Highscore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] TextMeshProUGUI scoreText;

    void Start()
    {
        int highscore = PlayerPrefs.GetInt("Highscore", 0);
        highscoreText.SetText("Highscore: " + highscore);

        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        scoreText.SetText("Score: " + currentScore);

        PlayerPrefs.DeleteKey("CurrentScore");
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
