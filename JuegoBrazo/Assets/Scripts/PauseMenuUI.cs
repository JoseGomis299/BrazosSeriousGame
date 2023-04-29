using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BugsGame
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private AudioClip buttonSound;

        private void Awake()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(() =>
                {
                    Resume();
                    AudioManager.Instance.PlaySound(buttonSound);
                });
            if (quitButton != null) quitButton.onClick.AddListener(HomeButtonAction);
            if (restartButton != null) restartButton.onClick.AddListener(RestartButtonAction);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            AudioManager.Instance.StopEffect();
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        private void Resume()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
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
