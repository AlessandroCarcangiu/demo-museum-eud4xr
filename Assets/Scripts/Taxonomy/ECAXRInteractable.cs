using System;
using System.Collections;
using ECARules4All_DLL;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Action = ECARules4All_DLL.Action;
using Behaviour = ECARules4All_DLL.Behaviour;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Behaviour))] //TODO Bug se non metti RequireComponent(typeof(Interactable))
    [RequireComponent(typeof(Interactable))]
    [ECARules4All("xrinteractable")]
    public class ECAXRInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
    {
        public string tagPlayer = string.Empty;
        // private bool interactionIsStarted = false;
        private float interactionStartTime;
        private float interactionDuration = 0.50f;

        /// <summary>
        /// <b>isInteracted</b> 
        /// </summary>
        [StateVariable("isInteracted", ECARules4AllType.Boolean)]
        public ECABoolean isInteracted
        {
            get => _isInteracted;
            set
            {
                _isInteracted = value;
                ECAScript.NotifyUpdate(this, nameof(isInteracted), isInteracted.ToString());
            }
        }

        private ECABoolean _isInteracted = new ECABoolean(ECABoolean.BoolType.NO);

        // Called when the object is first grabbed
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args); // Trigger default grab behavior

            Action a;
            GameObject subject = string.IsNullOrEmpty(tagPlayer)
                ? args.interactorObject.transform.gameObject
                : GameObject.FindWithTag("Player");

            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor
                || args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor
                || args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor)
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
            
            interactionStartTime = Time.time;
            // interactionIsStarted = true;
            StartCoroutine(CheckHoverDuration());
            
            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args); // Trigger default release behavior

            Action a;
            GameObject subject = string.IsNullOrEmpty(tagPlayer)
                ? args.interactorObject.transform.gameObject
                : GameObject.FindWithTag("Player");

            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor
                || args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor
                || args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor)
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
            
            // interactionIsStarted = false;
            if (isInteracted.Equals(ECABoolean.YES))
            {
                isInteracted = new ECABoolean(ECABoolean.BoolType.NO);
            }
            
            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            Action a;
            GameObject subject = string.IsNullOrEmpty(tagPlayer)
                ? args.interactorObject.transform.gameObject
                : GameObject.FindWithTag("Player");
            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor)
            {
                Debug.Log($"{gameObject.name} pointed at by ray.");
                //TODO Add the ECAMethods in ECACharacters.cs
                // a = new Action(subject, "points", this.gameObject);
                a = new Action(subject, "interacts with", this.gameObject);
            }
            else
            {
                throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
            }

            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

            Action a;
            GameObject subject = string.IsNullOrEmpty(tagPlayer)
                ? args.interactorObject.transform.gameObject
                : GameObject.FindWithTag("Player");
            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor)
            {
                a = new Action(subject, "stops-interacting with", this.gameObject);
                Debug.Log($"{gameObject.name} stopped pointing at by ray.");
            }
            else
            {
                throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
            }

            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
        }

        private IEnumerator CheckHoverDuration()
        {
            while (isHovered && isInteracted.Equals(ECABoolean.NO))
            {
                Debug.Log($"Time: {Time.time - interactionStartTime} >= {interactionDuration}");
                if (Time.time - interactionStartTime >= interactionDuration)
                {
                    isInteracted = new ECABoolean(ECABoolean.BoolType.YES);
                    Debug.Log("IS POINTED");
                }
                yield return null;
            }
        }
    }
}