using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Singletons
{
    public class PlayerUI : Singleton<PlayerUI>
    {
        [SerializeField]
        private TextMeshProUGUI playerScore;
        public int Score { get; private set; }

        [SerializeField]
        private Slider playerHealth;

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion Singleton

        // Start is called before the first frame update
        private void Start()
        {
            this.Score = 0;
            this.playerScore.SetText("Score: " + this.Score);
        }

        public void AddScore(int points)
        {
            this.Score += points;
            this.playerScore.SetText("Score: " + this.Score);


            int highscore = PlayerPrefs.GetInt(Highscore.TAG_HIGHSCORE, 0);
            if (this.Score > highscore)
            {
                highscore = this.Score;
                PlayerPrefs.SetInt(Highscore.TAG_HIGHSCORE, highscore);
            }
 
            PlayerPrefs.SetInt(Highscore.TAG_SCORE, this.Score);
            PlayerPrefs.Save();
        }

        public void SetHealth(float value)
        {
            if (this.playerHealth == null)
                return;

            this.playerHealth.value = value;
        }
    }
}