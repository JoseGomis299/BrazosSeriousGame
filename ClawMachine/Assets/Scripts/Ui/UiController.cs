using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
    {
        //[SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameOverMenu;

        //[SerializeField, GradientUsage(true)] private Gradient gradient;

        private bool _gameEnded;

        private void Start()
        {
            // GameManager.instance.onTimerChange += (time, timeAdded) =>
            // {
            //     int cSeconds = (int)(60 * (Mathf.Floor(time * 100) - Mathf.Floor(time) * 100) / 100);
            //     int seconds = (int)time;
            //
            //     var cSecondsString = cSeconds < 10 ? "0" + cSeconds : cSeconds.ToString();
            //     var secondsString = seconds < 10 ? "0" + seconds : seconds.ToString();
            //
            //     timerText.text = $"{secondsString}<size=90%>:<size=80%>{cSecondsString}";
            //     timerText.color = gradient.Evaluate(1 - (time / GameManager.instance.initialTime));
            // };
            GameManager.instance.onScoreChange += score =>
            {
                if(scoreText == null)return;
                if (_gameEnded) gameOverMenu.GetComponent<GameOverMenuUI>().SetScoreTexts(score);
                scoreText.text = score.ToString();
            };

            GameManager.instance.onGameEnded += () =>
            {
                _gameEnded = true;
                gameOverMenu.SetActive(true);
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
}


