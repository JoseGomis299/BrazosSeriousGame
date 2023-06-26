using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dartboard : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
   {
      var dart = collision.gameObject.GetComponent<DartCollisionDetection>();
      if(dart == null) return;
      
      GameManager.instance.ResetPositions();
   }
}
