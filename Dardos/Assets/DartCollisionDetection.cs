using System;
using System.Collections;
using System.Collections.Generic;
using ProjectUtils.ObjectPooling;
using UnityEngine;

public class DartCollisionDetection : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        Balloon balloon = other.GetComponent<Balloon>();
        if(balloon == null) { return; }
        
        balloon.gameObject.SetActive(false);
        GameManager.instance.AddScore(balloon.score);
        ObjectPool.Instance.InstantiateFromPoolIndex(2, transform.position, Quaternion.identity, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            GameManager.instance.DartThrown();
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
