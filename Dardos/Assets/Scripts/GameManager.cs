using System;
using System.Collections.Generic;
using System.Threading;
using ProjectUtils.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

        public static GameManager instance;
        public int score { get; private set; }
        [field:SerializeReference] public int dartCount { get; private set; }

        //public event Action<float, float> onTimerChange;
        public event Action<int> onScoreChange;
        public event Action onGameEnded;
        public event Action<int> onDartThrown; 
        private PathDrawer _pathDrawer;
        
        private void Awake()
        {
            if (instance != null) Destroy(gameObject);
            else instance = this;
            Time.timeScale = 1;
            _pathDrawer = GetComponent<PathDrawer>();
        }
        
        private void Start()
        {
            ResetPositions();
        }

        public void DartThrown()
        {
            if(--dartCount >= 0) onDartThrown?.Invoke(dartCount);
            if (dartCount <= 1) onGameEnded?.Invoke();
        }

        public void ResetPositions()
        {
            foreach (var balloon in FindObjectsOfType<Balloon>())
            {
                balloon.gameObject.SetActive(false);
            }
            foreach (var dart in FindObjectsOfType<DartCollisionDetection>())
            {
                dart.gameObject.SetActive(false);
            }
            _pathDrawer.DrawPath();
        }
        
        // private void Update()
        // {
        //     AddTime(-Time.deltaTime);
        // }

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

        // public void AddTime(float timeAdded)
        // {
        //     _timer += timeAdded;
        //     if (_timer <= 0)
        //     {
        //         _timer = 0;
        //         onTimerChange?.Invoke(_timer, timeAdded);
        //         if (score > PlayerPrefs.GetInt("bugsHighScore", 0)) PlayerPrefs.SetInt("bugsHighScore", score);
        //         onGameEnded?.Invoke();
        //         return;
        //     }
        //
        //     onTimerChange?.Invoke(_timer, timeAdded);
        // }

}

