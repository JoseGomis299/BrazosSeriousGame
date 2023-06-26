using System.Collections;
using System.Collections.Generic;
using ProjectUtils.ObjectPooling;
using UnityEngine;

public class BalloonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] balloonPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRate;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > spawnRate)
        {
            SpawnBalloon();
            _timer = Random.Range(0, spawnRate/3);
        }
    }

    private void SpawnBalloon()
    {
        int prefabIndex = Random.value >= 0.75f ? 1 : 0;
        ObjectPool.Instance.InstantiateFromPool(balloonPrefabs[prefabIndex], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, false);
    }
}
