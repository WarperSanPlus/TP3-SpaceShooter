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
            this.playerScore.SetText("Score: " + this.Score);
        }

        public void AddScore(int points)
        {
            this.Score += points;
            this.playerScore.SetText("Score: " + this.Score);
        }
    }
}