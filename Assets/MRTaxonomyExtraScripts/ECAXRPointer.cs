using System;
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using Oculus.Interaction;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects
{
    /// <summary>
    /// <b>ECAXRPointer</b> is a custom ECA component that makes the object owner pointable by the user represents an XR ray pointer interaction.
    /// This class exposes the state variable <b>isPointed</b>,
    /// while the actions <b>points to</b> and <b>stops-pointing to</b> are in the script <see cref="ECAObject"/>,
    /// which allow automations to reason about when a character points at this object with an XR ray.
    /// </summary>
    [ECARules4All("xrPointer")]
    [RequireComponent(typeof(ECABehaviour))]
    [RequireComponent(typeof(ECAInteractable))]
    [RequireComponent(typeof(InteractableUnityEventWrapper))]
    [DisallowMultipleComponent]
    public class ECAXRPointer : MonoBehaviour
    {

        private InteractableUnityEventWrapper _interactableUnityEventWrapper;
        [SerializeField] private RayInteractable _rayInteractable;
        private float hoverStartTime;

        private float
            hoverDuration =
                0.11f; // I know it's not used right now. We may want to use it in the future. The old code is in DemoMuseum


        private GameObject player_character => ECAPlayer_Singleton.Instance.playerGoRef;

        private void Awake()
        {
            if (_rayInteractable == null)
            {
                throw new Exception("RayInteractable is null. Cannot stop grabbing.");
            }

            _interactableUnityEventWrapper = this.GetComponent<InteractableUnityEventWrapper>();
            _interactableUnityEventWrapper.InjectInteractableView(_rayInteractable);
        }

        private void Start()
        {
            _interactableUnityEventWrapper.WhenHover.AddListener(() => InspectorStartsGrabbing(_rayInteractable));
            _interactableUnityEventWrapper.WhenUnhover.AddListener(() => InspectorStopsGrabbing(_rayInteractable));
        }

        /// <summary>
        /// <b>isPointed</b> indicates whether this object is currently being pointed at by the player character’s XR ray pointer.
        /// </summary>
        [ECARelevance(true)]
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

        [SerializeField] private ECABoolean _isPointed = new ECABoolean(ECABoolean.BoolType.NO);

        public void _StartsPointing(ECACharacter c)
        {
            Debug.Log("Starting grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
            isPointed = new ECABoolean(ECABoolean.BoolType.YES);
        }


        public void _StopsPointing(ECACharacter c)
        {
            Debug.Log("Stops grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
            isPointed = new ECABoolean(ECABoolean.BoolType.NO);
        }

        private void InspectorStartsGrabbing(RayInteractable rayInteractable)
        {
            // hoverStartTime = Time.time;
            // StartCoroutine(CheckHoverDuration());

            _StartsPointing(player_character.GetComponent<ECACharacter>());
            Action action = new Action(player_character, "points to", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
        }

        private void InspectorStopsGrabbing(RayInteractable g)
        {
            _StopsPointing(player_character.GetComponent<ECACharacter>());
            Action action = new Action(player_character, "stops-pointing to", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this, action);
        }

        // private IEnumerator CheckHoverDuration()
        // {
        //     while (isHovered && isPointed.Equals(ECABoolean.NO))
        //     {
        //         // Debug.Log($"Time: {Time.time - hoverStartTime} >= {hoverDuration}");
        //         if (Time.time - hoverStartTime >= hoverDuration)
        //         {
        //             isPointed = new ECABoolean(ECABoolean.BoolType.YES);
        //             Debug.Log("IS POINTED");
        //         }
        //         yield return null;
        //     }
        // }
    }
}