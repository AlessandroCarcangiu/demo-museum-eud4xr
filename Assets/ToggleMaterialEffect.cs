using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.Playables;

public class ToggleMaterialEffect : IEffect
{
    public float ActivationThreshold = 0.001f;
    
    public Renderer TargetRenderer;
    
    public Material MaterialOn;
    public Material MaterialOff;
    
    public bool isOn = false;
    public void Setup(PlayableGraph graph, GameObject owner)
    {
        if (TargetRenderer == null)
        {
            TargetRenderer = owner.GetComponent<Renderer>();
        }
        
        if (MaterialOn == null)
        {
            throw new Exception("MaterialOn is not set");
        }
        
        if (MaterialOff == null)
        {
            throw new Exception("MaterialOff is not set");
        }
        
        if (TargetRenderer == null)
        {
            throw new Exception("TargetRenderer is not set");
        }

        TargetRenderer.material = isOn ? MaterialOn : MaterialOff;
    }

    
    public bool Evaluate(float parameter)
    {
        var isOnThisFrame = parameter > ActivationThreshold;

        if (isOnThisFrame != isOn)
        {
            Debug.Log("CHANGE MATERIAL");
            if (isOnThisFrame)
            {
                TargetRenderer.material = MaterialOn;
            }
            else
            {
                TargetRenderer.material = MaterialOff;
            }
            
            isOn = isOnThisFrame;
        }

        return true; // We are always immediately done.
    }
}
