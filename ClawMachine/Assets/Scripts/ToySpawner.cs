using System.Collections.Generic;
using UnityEngine;

public class ToySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> toys;
    [SerializeField] private GameObject protector;
    [SerializeField] private int toyNumber = 15;
    void Start()
    {
        for (int i = 0; i < toyNumber; i++)
        {
          SpawnToy();   
        }
        Invoke(nameof(DisableProtector), 2f);
    }

    private void SpawnToy()
    {
        Vector3 pos = new Vector3(Random.Range(-1.2f, 0.5f), Random.Range(2, 1.5f), Random.Range(-1.2f, 0.5f));
        GameObject toy = GameObject.Instantiate(toys[Random.Range(0, toys.Count)], transform);
        toy.transform.localPosition = pos;
        toy.transform.rotation = Random.rotation;
    }

    private void DisableProtector()
    {
        protector.SetActive(false);
    }
}
