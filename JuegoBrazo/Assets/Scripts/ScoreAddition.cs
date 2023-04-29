using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
            if(score >= 100)  text.text = $"<color=\"yellow\">+{score}";
            else text.text = $"+{score}";
        } 
        else text.text = $"<color=\"red\">{score}";
        speed = initialMovementSpeed;
        transform.localScale = Vector3.one * (2 * Random.Range(1f, 1.1f));
        speed *= Random.Range(0.8f, 1.3f);
        text.color = initialColor;
        StartCoroutine(Transition(transitionDuration, score));
    }
    
    
    private IEnumerator Transition(float time, int score)
    {
        float timer = Time.unscaledDeltaTime;
        Vector3 initialScale = transform.localScale;
        Vector3 scaleDelta = initialScale * (Random.Range(0f, 0.5f) * (score/Mathf.Abs(score)));
        Vector3 direction = new Vector3(0,  Random.Range(0.8f,1f))* (score/Mathf.Abs(score));
        Color color = text.color;
        float initialAlpha = text.color.a;

        while (timer < time)
        {
            speed = initialMovementSpeed *(1 - (timer / time));
            transform.localScale = initialScale + scaleDelta * (timer/time);
            direction.x = Mathf.Sin(timer*5);
            transform.position += direction * (speed * Time.unscaledDeltaTime);
            color.a = initialAlpha - (timer/time);
            text.color = color;
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        gameObject.SetActive(false);
    }
}
