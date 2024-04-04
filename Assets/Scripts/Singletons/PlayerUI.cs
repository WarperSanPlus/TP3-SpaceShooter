using TMPro;

namespace Singletons
{
    public class PlayerUI : Singleton<PlayerUI>
    {
        private TextMeshProUGUI playerScore;
        public int Score { get; private set; }

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion Singleton

        // Start is called before the first frame update
        private void Start()
        {
            this.playerScore = this.GetComponentInChildren<TextMeshProUGUI>();
            this.Score = 0;
            this.playerScore.SetText("Score: " + this.Score);
        }

        public void AddScore(int points)
        {
            this.Score += points;
            this.playerScore.SetText("Score: " + this.Score);


            int highscore = PlayerPrefs.GetInt("Highscore", 0);
            if (this.Score > highscore)
            {
                highscore = this.Score;
                PlayerPrefs.SetInt("Highscore", highscore);
                PlayerPrefs.Save();
            }
 
            PlayerPrefs.SetInt("CurrentScore", this.Score);
            PlayerPrefs.Save();
        }
    }
}