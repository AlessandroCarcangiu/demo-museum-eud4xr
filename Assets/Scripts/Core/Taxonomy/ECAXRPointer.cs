using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL.Utils;
using MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Behaviour))] //TODO Bug se non metti RequireComponent(typeof(Interactable))
    [RequireComponent(typeof(ECAInteractable))]
    [ECARules4All("xrpointer")]
    public class ECAXRPointer : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
    {
        public string tagPlayer = "Player";
        // private bool isHover = false;
        private float hoverStartTime;
        private float hoverDuration = 0.11f;
        
        /// <summary>
        /// <b>isPointed</b> 
        /// </summary>
        [StateVariable("isPointed", ECARules4AllType.Boolean)]
        public ECABoolean isPointed
        {
            get => _isPointed;
            set
            {
                _isPointed = value;
                ECAScript.NotifyUpdate(this, nameof(isPointed), isPointed.ToString());
            }
        }
        private ECABoolean _isPointed = new ECABoolean(ECABoolean.BoolType.NO);

        private List<Type> hoverIgnoreInteractors = new() { typeof(GazePinchInteractor) };
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            Action a = null;
            GameObject subject = GameObject.FindWithTag("Player");

            if (args.interactorObject is XRRayInteractor)
            {
                if (args.interactorObject is GazeInteractor)
                {
                    // The GazeInteractor is a custom class that extends XRRayInteractor, do we want to handle it differently?
                    Debug.Log($"{gameObject.name} is looked passively (gaze).");
                    return;
                }
                else
                {
                    Debug.Log($"{gameObject.name} pointed at by ray by {subject}.");
                    //TODO Do we want to use interacts with or another verb?. In case you need to add the ECAMethods in ECACharacters.cs
                    // a = new Action(subject, "points", this.gameObject);
                    a = new Action(subject, "points", this.gameObject);                    
                }
            }
            else if (hoverIgnoreInteractors.Contains(args.interactorObject.GetType()))
            {
                Debug.Log($"Ignoring hover-enter with interactor {args.interactorObject}");
                return;
            }
            else {
                throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
            }
            
            if (a == null)
            {
                throw new Exception("Action is null, something went wrong.");
            }
            hoverStartTime = Time.time;
            // isHover = true;
            StartCoroutine(CheckHoverDuration());
            
            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
            Debug.Log("Start HOVER");
        }
        
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

            Action a = null;
            GameObject subject = string.IsNullOrEmpty(tagPlayer)
                ? args.interactorObject.transform.gameObject
                : GameObject.FindWithTag("Player");
            if (args.interactorObject is XRRayInteractor)
            {
                if (args.interactorObject is GazeInteractor)
                {
                    // The GazeInteractor is a custom class that extends XRRayInteractor, do we want to handle it differently?
                    Debug.Log($"{gameObject.name} stopped looked at passively (gaze).");
                    return;
                }
                else
                {
                    a = new Action(subject, "stops-pointing", this.gameObject);
                    Debug.Log($"{gameObject.name} stopped pointing at by ray.");   
                }
            }
            else if (hoverIgnoreInteractors.Contains(args.interactorObject.GetType()))
            {
                Debug.Log($"Ignoring hover-exit with interactor {args.interactorObject}");
                return;
            }
            else
            {
                throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
            }

            if (a == null)
            {
                throw new Exception("Action is null, something went wrong.");
            }
            // isHover = false;
            if (isPointed.Equals(ECABoolean.YES))
            {
                isPointed = new ECABoolean(ECABoolean.BoolType.NO);
            }
            
            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
            Debug.Log("Exit HOVER");
        }
        
        private IEnumerator CheckHoverDuration()
        {
            while (isHovered && isPointed.Equals(ECABoolean.NO))
            {
                // Debug.Log($"Time: {Time.time - hoverStartTime} >= {hoverDuration}");
                if (Time.time - hoverStartTime >= hoverDuration)
                {
                    isPointed = new ECABoolean(ECABoolean.BoolType.YES);
                    Debug.Log("IS POINTED");
                }
                yield return null;
            }
        }
    }
}