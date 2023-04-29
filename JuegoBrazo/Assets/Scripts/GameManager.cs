using System;
using BugsGame.ProjectUtils.Helpers;
using UnityEngine;

namespace BugsGame
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance;
        public int score { get; private set; }

        [field: SerializeReference] public float initialTime { get; private set; } = 60;
        private float _timer;

        public event Action<float, float> onTimerChange;
        public event Action<int> onScoreChange;
        public event Action onGameEnded;

        public Vector4 bounds { get; private set; }

        private void Awake()
        {
            if (instance != null) Destroy(gameObject);
            else instance = this;

            var bottomBound = Helpers.Camera.ScreenToWorldPoint(new Vector3(0, 0, Screen.height)).z;
            var rightBound = Helpers.Camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
            var topBound = -bottomBound;
            var leftBound = -rightBound;
            bounds = new Vector4(rightBound, leftBound, topBound, bottomBound);
            _timer = initialTime;
            Time.timeScale = 1;
        }

        private void Update()
        {
            AddTime(-Time.deltaTime);
        }

        public void AddScore(int scoreAdded)
        {
            if (score + scoreAdded < 0)
            {
                score = 0;
                onScoreChange?.Invoke(score);
                return;
            }

            score += scoreAdded;
            onScoreChange?.Invoke(score);
        }

        public void AddTime(float timeAdded)
        {
            _timer += timeAdded;
            if (_timer <= 0)
            {
                _timer = 0;
                onTimerChange?.Invoke(_timer, timeAdded);
                if (score > PlayerPrefs.GetInt("bugsHighScore", 0)) PlayerPrefs.SetInt("bugsHighScore", score);
                onGameEnded?.Invoke();
                return;
            }

            onTimerChange?.Invoke(_timer, timeAdded);
        }

    }
}
