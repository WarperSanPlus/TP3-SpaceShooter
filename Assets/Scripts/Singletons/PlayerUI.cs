using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Singletons
{
    public class PlayerUI : Singleton<PlayerUI>
    {
        TextMeshProUGUI playerScore;
        public int Score { get; private set; }
        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            playerScore = GetComponentInChildren<TextMeshProUGUI>();
            playerScore.SetText("Score: " + Score);
        }

        public void AddScore(int points)
        {
            Score += points;
            playerScore.SetText("Score: " + Score);
        }
    }

}
