using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BugsGame
{
    public class GameOverMenuUI : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text highScoreText;

        [SerializeField] private AudioClip buttonSound;

        private void Awake()
        {
            if (restartButton != null) restartButton.onClick.AddListener(RestartButtonAction);
            if (quitButton != null) quitButton.onClick.AddListener(HomeButtonAction);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            restartButton.Select();
            Time.timeScale = 0;
            SetScoreTexts(GameManager.instance.score);
        }

        public void SetScoreTexts(int score)
        {
            scoreText.text = "SCORE: " + GameManager.instance.score;
            highScoreText.text = "HIGH SCORE: " + PlayerPrefs.GetInt("bugsHighScore", 0);
        }

        private void HomeButtonAction()
        {
            AudioManager.Instance.PlaySound(buttonSound);
            quitButton.enabled = false;
            SceneManager.LoadScene(0);
        }

        private void RestartButtonAction()
        {
            AudioManager.Instance.PlaySound(buttonSound);
            restartButton.enabled = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
