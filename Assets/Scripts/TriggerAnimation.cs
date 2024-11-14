using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAnimation : MonoBehaviour
{
    [Header("Custom Event")]
    public UnityEvent myEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (myEvents == null)
        {
            print("myEventTriggerOnEnter was triggered but myEvents was null");
        }

        else
        {
            if(other.CompareTag("Hand")){ 
                print("myEventTriggerOnEnter Activated. Triggering" + myEvents);
                myEvents.Invoke();
            }
        }
    }
}