using System.Collections;
using System.Collections.Generic;
using ProjectUtils.ObjectPooling;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform finalPosition;
    [SerializeField] private int steps;
    [SerializeField] private float maxHeight;
    [SerializeField] private GameObject balloonPrefab;

    private int _steps;
    private float _maxHeight;
    
    public void DrawPath()
    {
        finalPosition.parent.position = Vector3.forward*Random.Range(60, 111);
        _steps = steps + Random.Range(-1, 3);
        _maxHeight = maxHeight * finalPosition.parent.position.z / 110;
        CalculateTrajectory();
    }

    private void CalculateTrajectory()
    {
        Vector3 direction = finalPosition.position - startPosition.position;
        direction.Normalize();
        float distance = Vector3.Distance(startPosition.position, finalPosition.position);
        
        for (int i = 1; i < _steps+1; i++)
        {
            float percentage = (float) i / (_steps+1);
            Vector3 pos = startPosition.position + direction * distance * percentage;
            pos.y += _maxHeight * Mathf.PingPong(percentage * 2, 1);
            ObjectPool.Instance.InstantiateFromPool(balloonPrefab, pos, Quaternion.identity, false);
        }
    }
}
