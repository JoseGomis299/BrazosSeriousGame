using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DartsCount : MonoBehaviour
{
    [SerializeField] private Sprite dartSprite;
    private int dartCount;
    private GameObject[] darts;
    void Start()
    {
        dartCount = GameManager.instance.dartCount;
        darts = new GameObject[dartCount];
        GameObject dart = new GameObject();
        dart.AddComponent<CanvasRenderer>();
        dart.AddComponent<Image>();
        dart.GetComponent<Image>().sprite = dartSprite;
        for (int i = 0; i < dartCount; i++)
        {
            darts[i] = Instantiate(dart, transform);
        }

        GameManager.instance.onDartThrown += refreshUI;
    }

    public void refreshUI(int health)
    {
        for (int i = 0; i < health; i++)
        {
            darts[i].GetComponent<Image>().color = Color.white;
        }
        
        Color color = Color.white;
        color.a = 0.7f;
        
        for (int i = dartCount-1; i >= health; i--)
        {
            darts[i].GetComponent<Image>().color = color;
        }
    }
}
