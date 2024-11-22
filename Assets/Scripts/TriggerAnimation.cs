using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.Events;
using Action = ECARules4All_DLL.Action;

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
    
    /*
     // TEST
    private void Update()
    {
        GameObject hand = GameObject.Find("Hand");
        GameObject ophelia = GameObject.Find("DA_Ophelia");
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(hand);
            Debug.Log(ophelia);
            RuleEngine.GetInstance()
                .ExecuteAction(new Action(hand, "interacts with", ophelia));
            Debug.Log("Tasto AAAAA premuto");
        }
    }*/
}