using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BugsGame
{
    public class ScoreAddition : MonoBehaviour
    {
        [SerializeField] private float initialMovementSpeed;
        [SerializeField] private float transitionDuration;
        [SerializeField] private Color initialColor;
        private float speed;
        private TMP_Text text;

        public void SetStats(int score)
        {
            text = GetComponent<TextMeshProUGUI>();
            if (score > 0)
            {
                if (score >= 100) text.text = $"<color=\"yellow\">+{score}";
                else text.text = $"+{score}";
            }
            else text.text = $"<color=\"red\">{score}";

            speed = initialMovementSpeed;
            transform.localScale = Vector3.one * Random.Range(1.6f, 1.3f);
            speed *= Random.Range(0.8f, 1.3f);
            text.color = initialColor;
            StartCoroutine(Transition(transitionDuration, score));
        }
        
        public void SetStatsTime(int time)
        {
            text = GetComponent<TextMeshProUGUI>();
            text.text = time > 0 ? $"<color=\"green\">+{time}" : $"<color=\"red\">{time}";

            speed = initialMovementSpeed;
            transform.localScale = Vector3.one * Random.Range(2.5f, 3.5f);
            speed *= Random.Range(1.5f, 2f);
            text.color = initialColor;
            StartCoroutine(Transition(transitionDuration, time));
        }


        private IEnumerator Transition(float time, int score)
        {
            float timer = Time.unscaledDeltaTime;
            Vector3 initialScale = transform.localScale;
            Vector3 scaleDelta = initialScale * (Random.Range(0f, 0.5f) * (score / Mathf.Abs(score)));
            Vector3 direction = new Vector3(0, Random.Range(0.8f, 1f)) * (score / Mathf.Abs(score));
            Color color = text.color;
            float initialAlpha = text.color.a;

            while (timer < time)
            {
                speed = initialMovementSpeed * (1 - (timer / time));
                transform.localScale = initialScale + scaleDelta * (timer / time);
                direction.x = Mathf.Sin(timer * 5);
                transform.position += direction * (speed * Time.unscaledDeltaTime);
                color.a = initialAlpha - (timer / time);
                text.color = color;
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            gameObject.SetActive(false);
        }

    }
}
