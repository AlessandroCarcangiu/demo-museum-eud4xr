using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;
using Serilog;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Action = ECARules4All_DLL.Action;
using Behaviour = UnityEngine.Behaviour;

namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Behaviour))] //TODO Bug se non metti RequireComponent(typeof(Interactable))
    [RequireComponent(typeof(Interactable))]
    [ECARules4All("xrpointer")]
    public class ECAXRPointer : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
    {
        public string tagPlayer = string.Empty;
        private bool isHover = false;
        private float hoverStartTime;
        private float hoverDuration = 0.50f;
        
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
                
                //TODO Do we want to use interacts with or another verb?. In case you need to add the ECAMethods in ECACharacters.cs
                // a = new Action(subject, "points", this.gameObject);
                a = new Action(subject, "points", this.gameObject);
            }
            else {
                throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
            }
            
            hoverStartTime = Time.time;
            isHover = true;
            StartCoroutine(CheckHoverDuration());
            
            EventBus.GetInstance().Publish(a);
            ECAScript.NotifyUpdate(this, a);
            Debug.Log("Start HOVER");
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
                a = new Action(subject, "stops-pointing", this.gameObject);
                Debug.Log($"{gameObject.name} stopped pointing at by ray.");
            }
            else
            {
                throw new Exception($"Unknown/Not Handled interactor: {args.interactorObject.GetType()}");
            }

            isHover = false;
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
                Debug.Log($"Time: {Time.time - hoverStartTime} >= {hoverDuration}");
                if (Time.time - hoverStartTime >= hoverDuration)
                {
                    isPointed = new ECABoolean(ECABoolean.BoolType.YES);
                    Debug.Log("IS POINTED");
                }
                yield return null;
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
}