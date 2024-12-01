using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Action = ECARules4All_DLL.Action;

[RequireComponent(typeof(Behaviour))] //TODO Bug se non metti RequireComponent(typeof(Interactable))
[RequireComponent(typeof(Interactable))]
[DisallowMultipleComponent]
public class ECAXRPointer : XRBaseInteractable
{
    public string tagPlayer = string.Empty;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        
        base.OnHoverEntered(args);

        Action a;
        GameObject subject = string.IsNullOrEmpty(tagPlayer)
            ? args.interactorObject.transform.gameObject
            : GameObject.FindWithTag("Player");

        if (args.interactorObject is XRRayInteractor)
        {
            Debug.Log($"{gameObject.name} pointed at by ray.");
            
            //TODO Do we want to use interacts with or another verb?. In case you need to add the ECAMethods in ECACharacters.cs
            // a = new Action(subject, "points", this.gameObject);
            a = new Action(subject, "interacts with", this.gameObject);
        }
        else {
            throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
        }
    }
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        Action a;
        GameObject subject = string.IsNullOrEmpty(tagPlayer)
            ? args.interactorObject.transform.gameObject
            : GameObject.FindWithTag("Player");

        
        if (args.interactorObject is XRRayInteractor)
        {
            Debug.Log($"{gameObject.name} stopped pointing at by ray.");
        }
        else
        {
            throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
        }
    }

    //TODO Do we want/need to use these methods?
    // protected override void OnSelectEntered(SelectEnterEventArgs args)
    // {
    //     base.OnSelectEntered(args);
    //
    //     if (args.interactorObject is XRDirectInteractor)
    //     {
    //         Debug.Log($"{gameObject.name} grabbed by hand.");
    //         // Custom grab logic
    //     }
    // }
    //
    // protected override void OnSelectExited(SelectExitEventArgs args)
    // {
    //     base.OnSelectExited(args);
    //
    //     if (args.interactorObject is XRDirectInteractor)
    //     {
    //         Debug.Log($"{gameObject.name} released.");
    //         // Custom release logic
    //     }
    // }
}
