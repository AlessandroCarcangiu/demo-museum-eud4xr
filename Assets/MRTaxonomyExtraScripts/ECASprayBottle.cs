using System;
using ECARules4All_DLL.Taxonomies.Objects.Characters;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.Taxonomies.Objects.Props.Subcategories.LiquidDispenser.Subcategories{
/// <summary>
/// <b>ECASprayBottle</b> is a component that represents a virtual spray bottle capable of detecting pinch gestures
/// to trigger a spray action. The action is activated when both the index and middle fingers pinch
/// beyond a configurable threshold, causing the bottle to emit a spray and potentially trigger related ECA automation events.
/// </summary>
[ECARules4All("sprayBottle")]
[RequireComponent(typeof(ECALiquidDispenser))]
[DisallowMultipleComponent]
public class ECASprayBottle : MonoBehaviour
{
    private OVRHand rightHand;

    public float thrIndex = 0.4f;
    public float thrMiddle = 0.2f;
    public AudioSource audioSource;

    private ECALiquidSpawner _waterSpawner;

    private GameObject player_character => ECAPlayer_Singleton.Instance.playerGoRef;


    private void Awake()
    {
        _waterSpawner = this.GetComponent<ECALiquidDispenser>().liquidSpawner;
        if (_waterSpawner == null)
        {
            throw new Exception("[ECASprayBottle - Awake] ERROR: liquidSpawner == null");
        }

        // find gameobject RightHandAnchor and then search for the children named "[BuildingBlock] Hand Tracking right" and get the script OVRHand
        rightHand = GameObject.Find("RightHandAnchor").transform.Find("[BuildingBlock] Hand Tracking right")
            .GetComponent<OVRHand>();
        if (rightHand == null)
            throw new Exception("[ECASprayBottle - Awake] QUALCOSA E' ANDATO STORTO NON TROVO LA MANO");
    }


	/// <summary>
	/// <b>sprays</b> represents the action of dispensing liquid from an object equipped with an <see cref="ECASprayBottle"/> component,
	/// typically triggered by a character performing a pinch gesture with the hand.
	/// When executed, it releases a burst of liquid, plays an associated audio cue, and may trigger automation events
	/// related to cleaning, wetting, or environmental interactions.
	/// </summary>
	/// <param name="c">The object equipped with an <see cref="ECACharacter"/> component performing the spray action.</param>
    [Action(typeof(ECACharacter), "sprays", typeof(ECASprayBottle))]
    [ECARelevance(true)]
    public void _Sprays(ECACharacter c)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
        _waterSpawner.SpawnLiquid(ECALiquidSpawner.LiquidTemperature.Ambient);
    }

    // Update is called once per frame
    void Update()
    {
        /*float indexStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float middleStrength = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

        var indexColor = indexStrength > thrIndex ? "green" : "red";
        var middleColor = middleStrength > thrMiddle ? "green" : "red";

        bool isSpray = indexColor == "green" && middleColor == "green";
        if (isSpray)
        {
            // if (!audioSource.isPlaying)
            //     audioSource.Play();
            // liquidSpawner.SpawnWater(WaterSpawner.WaterType.Cold);
            _Sprays(player_character.GetComponent<ECACharacter>());
            var action = new Action(player_character, "sprays", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?                
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            _waterSpawner.StopSpawningLiquid();
        }*/
    }

    private void OnGUI()
    {
        // create button to spray water
        /*if (GUI.Button(new Rect(10, 10, 100, 30), "Spray Water"))
        {
            _Sprays(player_character.GetComponent<ECACharacter>());
            var action = new Action(player_character, "sprays", this.gameObject);
            EventBus.GetInstance().Publish(action);
            ECAScript.NotifyUpdate(this,
                action); //TODO J 2nd July '25: Is it necessary to notify the update here? Isn't automatic inside the EventBus?                
        }*/
    }
}
}