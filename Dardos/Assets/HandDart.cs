using System;
using System.Collections;
using System.Collections.Generic;
using ProjectUtils.ObjectPooling;
using UnityEngine;

public class HandDart : MonoBehaviour
{
    private Animator _animator;
    private float _rotationTimer;
    private float _timer;
    [SerializeField] private float rotationSpeed;
    private bool _prepared;
    private float _stopPreparingTime;
    private Transform _dartTransform;
    [SerializeField] private GameObject dartPrefab;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _stopPreparingTime = float.MinValue;
        _dartTransform = transform.GetChild(0);
    }

    private void Update()
    {
        RotateDart();
        HandleInput();
    }

    private void RotateDart()
    {
        _timer += Time.deltaTime;
        if (ArmInput.GetSignal(ArmInput.Signal.RBiceps) == 1 && _timer > 0.1f)
        {
            _rotationTimer += Time.deltaTime * rotationSpeed;
            Vector3 rotation = transform.eulerAngles;
            rotation.x = -1*Mathf.PingPong(_rotationTimer, 20);
            transform.eulerAngles = rotation;
        }
    }

    private void HandleInput()
    {
        if (ArmInput.GetSignal(ArmInput.Signal.RBiceps) == 1)
        {
            _prepared = true;
            _stopPreparingTime = Time.time;
            _animator.SetBool("Prepared", _prepared);
            return;
        }

        _timer = 0;
        _prepared = Time.time - _stopPreparingTime < 0.5f;
        _animator.SetBool("Prepared", _prepared);

        if (ArmInput.GetSignalDown(ArmInput.Signal.RTriceps) && _prepared)
        {
            float velocity = 0.84f / ((Time.time - _stopPreparingTime) / 10);
            velocity = Mathf.Clamp(velocity, 0, 200);
           // Debug.Log((Time.time - _stopPreparingTime) / 10);
           //  Debug.Log(velocity);
            
            GameObject dart = ObjectPool.Instance.InstantiateFromPool(dartPrefab, transform.position, transform.rotation, false);
            Rigidbody dartRb = dart.GetComponent<Rigidbody>();
            dartRb.transform.Rotate(_dartTransform.right, 180);

            dartRb.AddForce(-_dartTransform.forward*velocity, ForceMode.Impulse);
            dartRb.AddTorque(Vector3.right/80, ForceMode.Impulse);
            _stopPreparingTime -= 10;
            
        }   
    }
}
