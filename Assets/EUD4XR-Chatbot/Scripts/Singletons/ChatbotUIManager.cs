using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ECARules4All_DLL.Utils;

public class ChatbotUIManager : Singleton<ChatbotUIManager>
{
    public TextMeshProUGUI message;
    public TMP_Dropdown d_micList; //TODO Rimuoverlo in futuro. Direi di lasciarlo fino alla 1a build per vedere meglio quali microfoni ci sono nel visreo

    public AudioSource speaker;
    
    public FeedbackGenerator feedbackGenerator;
    public event Action OnAudioPlaybackCompleted;
    
    private void Start()
    {
        const string defaultOption = "Select Microphone";
        void DrawDropdownMicOptions()
        {
            d_micList.ClearOptions();
            d_micList.options.Add(new TMP_Dropdown.OptionData(defaultOption));
#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
            foreach (var device in Microphone.devices)
            {
                d_micList.options.Add(new TMP_Dropdown.OptionData(device));
            }
#endif
        }
        DrawDropdownMicOptions();

            
        void OnDropdownValueChanged(int index)
        {
            if (index == 0) return; // First option is just a label
            
            PlayerPrefs.SetInt("user-mic-device-index", index);
            string newMic = Microphone.devices[index];
            //MicrophoneManager.Instance.ChangeMicrophone(newMic);
        }
        
        var index = PlayerPrefs.GetInt("user-mic-device-index", 1);
        
        Debug.Log($"[ChatbotUIManager - Start] Microphone.devices.Length: {Microphone.devices.Length} - " +
                  $"Microphone.devices: {Microphone.devices} - index: {index}");
        
        //string mic = Microphone.devices[index]; // ?
        
        //MicrophoneManager.Instance.ChangeMicrophone(mic);
        d_micList.SetValueWithoutNotify(index);
        
        if (feedbackGenerator == null)
        {
            throw new Exception("FeedbackGenerator not set in ChatbotUIManager");
        }
        
        d_micList.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void UpdateTranscription(string s)
    {
        message.text = s;
    }

    
    public void SpeakTranscription(AudioClip clip, Action adHocCallback = null)
    {
        speaker.clip = clip;
        speaker.Play();

        List<Action> callbacks = new List<Action>() {() => OnAudioPlaybackCompleted?.Invoke()};
        if (adHocCallback != null)
            callbacks.Add(adHocCallback);
        
        StartCoroutine(CheckAudioPlaybackCompletion(callbacks));
    }
    
    private IEnumerator CheckAudioPlaybackCompletion(List<Action> callbackList)
    {
        // Wait for the audio to start playing
        if (!speaker.isPlaying)
            yield return new WaitUntil(() => !speaker.isPlaying);
        
        // Now wait for the audio to finish playing        
        yield return new WaitUntil(() => !speaker.isPlaying);
        
        // Invoke the callbacks
        callbackList.ForEach(c => c?.Invoke());
    }
    
    public void StopSpeaking()
    {
        speaker.Stop();
    }

    private const string EXPORT_AGENT_LABEL = "exportAgent"; //TODO Fare qualcosa per la taskModellingExportAgent?
    public void FeedbackAutomationCreated(string currNode)
    {
        if (currNode == EXPORT_AGENT_LABEL)
        {
            Debug.Log("Exporting agent");
            feedbackGenerator.PlayFeedback();
        }
    }

    
    [ContextMenu("TEST PARTICLE")]
    void TestParticle()
    {
        FeedbackAutomationCreated(EXPORT_AGENT_LABEL);
    }
}