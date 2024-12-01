using System;
using ECARules4All_DLL;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Action = ECARules4All_DLL.Action;
using Behaviour = ECARules4All_DLL.Behaviour;

[RequireComponent(typeof(Behaviour))]  //TODO Bug se non metti RequireComponent(typeof(Interactable))
[RequireComponent(typeof(Interactable))]
[DisallowMultipleComponent]
public class ECAXRInteractable : XRGrabInteractable
{
    public string tagPlayer = string.Empty;

    private void Start()
    {
        throw new NotImplementedException();
    }

    // Called when the object is first grabbed
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args); // Trigger default grab behavior

        Action a;
        GameObject subject = string.IsNullOrEmpty(tagPlayer)
            ? args.interactorObject.transform.gameObject
            : GameObject.FindWithTag("Player");

        if (args.interactorObject is XRRayInteractor
            || args.interactorObject is XRDirectInteractor
            || args.interactorObject is XRPokeInteractor)
        {
            Debug.Log("Pointer Down: Object grabbed by ray");

            //TODO Add the ECAMethods in ECACharacters.cs
            // a = new Action(subject, "selects", this.gameObject);
            a = new Action(subject, "interacts with", this.gameObject);
        }
        else
        {
            throw new Exception("Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
        }

        EventBus.GetInstance().Publish(a);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args); // Trigger default release behavior

        Action a;
        GameObject subject = string.IsNullOrEmpty(tagPlayer)
            ? args.interactorObject.transform.gameObject
            : GameObject.FindWithTag("Player");

        if (args.interactorObject is XRRayInteractor
            || args.interactorObject is XRDirectInteractor
            || args.interactorObject is XRPokeInteractor)
        {
            Debug.Log("Pointer Up: Object released by ray");

            //TODO Add the ECAMethods in ECACharacters.cs
            // a = new Action(subject, "stops selecting", this.gameObject);
            a = new Action(subject, "stops-interacting with", this.gameObject);
        }
        else
        {
            throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
        }

        EventBus.GetInstance().Publish(a);
    }
    
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
            
            //TODO Add the ECAMethods in ECACharacters.cs
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

}