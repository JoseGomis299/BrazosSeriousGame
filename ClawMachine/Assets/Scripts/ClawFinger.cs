using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClawFinger : MonoBehaviour
{
    public Quaternion initialRotation;
    public Quaternion finalRotation;
    public Quaternion openRotation;

    [HideInInspector] public Transform grabbed;
    public enum RotationType
    {
        initial, final, open
    }
    private bool _stopRotating;

    public async Task DoRotateAsync(Quaternion targetRotation, float time, bool useCondition)
    {
        _stopRotating = false;
        float timer = Time.deltaTime;
        Quaternion rotation = transform.localRotation;

        while (timer < time && (!useCondition || !_stopRotating))
        {
            transform.localRotation = Quaternion.Slerp(rotation, targetRotation, timer/time);
            await Task.Yield();
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.Equals(grabbed)) _stopRotating = true;
    }

    public Quaternion GetRotation(RotationType rotationType)
    {
        switch (rotationType)
        {
            case RotationType.open: return openRotation;
            case RotationType.final: return finalRotation;
        }
        return initialRotation;
    }
}
