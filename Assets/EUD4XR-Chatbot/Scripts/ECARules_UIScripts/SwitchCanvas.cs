using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCanvas : MonoBehaviour
{
    // todo forse non serve, cancellare?
    public List<Canvas> canvases;
    public int tabSelected = 0;
    void Start()
    {
        // Sanity check
        if (canvases.Count == 0)
            throw new System.ArgumentException("canvases", "canvases must have at least one element");
        
        if (tabSelected < 0 || tabSelected >= canvases.Count)
            throw new System.ArgumentOutOfRangeException("tabSelected", tabSelected, "tabSelected must be between 0 and " + (canvases.Count - 1));
        
        
        // Enable only the selected tab
        for (int i = 0; i < canvases.Count; i++)
            canvases[i].gameObject.SetActive(i == tabSelected);        
    }
}
