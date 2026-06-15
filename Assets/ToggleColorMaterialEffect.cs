using System;
using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.Playables;

public class ToggleColorMaterialIEffect : IEffect
{
    public float ActivationThreshold = 0.001f;
    
    public Renderer TargetRenderer;
    
    public Color ColorOn;
    public Color ColorOff;
    
    public bool isOn = false;
    public void Setup(PlayableGraph graph, GameObject owner)
    {
        if (TargetRenderer == null)
        {
            TargetRenderer = owner.GetComponent<Renderer>();
        }
        
        if (TargetRenderer == null)
        {
            throw new Exception("TargetRenderer is not set");
        }

        TargetRenderer.material.color = isOn ? ColorOn : ColorOff;
        
        
    }

    
    public bool Evaluate(float parameter)
    {
        var isOnThisFrame = parameter > ActivationThreshold;

        if (isOnThisFrame != isOn)
        {
            Debug.Log("CHANGE MATERIAL");
            TargetRenderer.material.color = isOnThisFrame ? ColorOn : ColorOff;
            
            isOn = isOnThisFrame;
        }

        return true; // We are always immediately done.
    }
}
