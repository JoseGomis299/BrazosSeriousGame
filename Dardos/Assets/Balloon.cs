using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Balloon : MonoBehaviour
{
    public int score;

    [SerializeField] private float speed = 0.15f;
    [SerializeField] private float frequency = 3;
    [SerializeField] private float amplitude = 0.02f;
    private float _speed;
    private float _frequency;
    private float _amplitude;
    private float _direction;
    private float _offset;

    private void OnEnable()
    {
        if (transform.localPosition.x > 0) _direction = -1;
        else _direction = 1;
        
        _speed = speed * Random.Range(0.75f, 1.5f);
        _amplitude = amplitude * Random.Range(0.75f, 1.5f);
        _frequency = frequency * Random.Range(0.75f, 1.5f);
        _offset=Random.Range(-0.1f, 0.1f);
    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector3(_direction*_speed, Mathf.Sin(2*Mathf.PI*Time.time*_frequency+_offset)*_amplitude));
    }
}
