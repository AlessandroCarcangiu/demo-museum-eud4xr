using System;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories{
/// <summary>
/// <b>ECAXRGrabbable</b> is a component that makes an object interactable and grabbable in XR within the ECA framework.
/// It ensures that grab and release events are properly mapped into the ECA system,
/// allowing automations to respond to physical interactions.
/// This component exposes the state variable <b>grabbed</b> and the actions <b>starts-grabbing</b> and <b>stops-grabbing</b>,
/// which enable automation rules to reason about when a character begins or ends physical interaction with the object.
/// </summary>
[ECARules4All("xrGrabbable")]
[RequireComponent(typeof(ECABehaviour))]
[RequireComponent(typeof(ECAInteractable))]
[RequireComponent(typeof(InteractableUnityEventWrapper))]
[DisallowMultipleComponent]
public class ECAXRGrabbable : MonoBehaviour
{
    
    private InteractableUnityEventWrapper _interactableUnityEventWrapper;
    [SerializeField] private HandGrabInteractable _grabbable;
    private GameObject player_character => ECAPlayer_Singleton.Instance.playerGoRef;
    
    private void Awake()
    {
        if (_grabbable == null)
        {
                throw new Exception("Grabbable is null. Cannot stop grabbing.");
        }
        _interactableUnityEventWrapper = this.GetComponent<InteractableUnityEventWrapper>();
        _interactableUnityEventWrapper.InjectInteractableView(_grabbable);
    }

    private void Start()
    {
        _interactableUnityEventWrapper.WhenSelect.AddListener(() => InspectorStartsGrabbing(_grabbable));
        _interactableUnityEventWrapper.WhenUnselect.AddListener(() => InspectorStopsGrabbing(_grabbable));
    }

    /// <summary>
    /// <b>grabbed</b> indicates whether the object is currently being held by the player character.
    /// </summary>
    [ECARelevance(true)]
    [StateVariable("grabbed", ECARules4AllType.Boolean)]
    public ECABoolean grabbed
    {
        get => _grabbed;
        set
        {
            _grabbed = value;
            ECAScript.NotifyUpdate(this, nameof(grabbed), grabbed.ToString());
        }
    }

    [SerializeField] private ECABoolean _grabbed = new ECABoolean(ECABoolean.BoolType.NO);

    /// <summary>
    /// <b>StartsGrabbing</b> is an action that occurs when the player character begins holding this object.
    /// </summary>
    /// <param name="c">The <see cref="ECACharacter"/> that performs the grabbing action.</param>
    [ECARelevance(true)]
    [Action(typeof(ECACharacter), "starts-grabbing")]
    public void _StartsGrabbing(ECACharacter c)
    {
        Debug.Log("Starting grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
        grabbed = new ECABoolean(ECABoolean.BoolType.YES);
    }

    /// <summary>
    /// <b>StopsGrabbing</b> is an action that occurs when the player character releases this object.
    /// </summary>
    /// <param name="c">The <see cref="ECACharacter"/> that performs the releasing action.</param>
    [ECARelevance(true)]
    [Action(typeof(ECACharacter), "stops-grabbing")]
    public void _StopsGrabbing(ECACharacter c)
    {
        Debug.Log("Stops grabbing with " + c.gameObject.name + " on " + this.gameObject.name);
        grabbed = new ECABoolean(ECABoolean.BoolType.NO);
    }

    private void InspectorStartsGrabbing(HandGrabInteractable g)
    {
        // Character c = GameObject.FindWithTag("Player").GetComponent<Character>();
        _StartsGrabbing(player_character.GetComponent<ECACharacter>());
        Action action = new Action(player_character, "starts-grabbing", this.gameObject);
        EventBus.GetInstance().Publish(action);
        ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
    }

    private void InspectorStopsGrabbing(HandGrabInteractable g)
    {
        // Character c = GameObject.FindWithTag("Player").GetComponent<Character>();
        _StopsGrabbing(player_character.GetComponent<ECACharacter>());
        Action action = new Action(player_character, "stops-grabbing", this.gameObject);
        EventBus.GetInstance().Publish(action);
        ECAScript.NotifyUpdate(this, action); //TODO J 1st July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?
    }
}
}