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
    

    public event Action OnAudioPlaybackCompleted;
    private void Start()
    {
        void DrawDropdownMicOptions()
        {
            d_micList.ClearOptions();
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
            PlayerPrefs.SetInt("user-mic-device-index", index);
            string newMic = Microphone.devices[index];
            MicrophoneManager.Instance.ChangeMicrophone(newMic);
        }
        d_micList.onValueChanged.AddListener(OnDropdownValueChanged);

        var index = PlayerPrefs.GetInt("user-mic-device-index", 1);
        string mic = Microphone.devices[index];
        MicrophoneManager.Instance.ChangeMicrophone(mic);
        d_micList.SetValueWithoutNotify(index);
        

    }

    public void UpdateTranscription(string s)
    {
        message.text = s;
    }

    private void SpeakTranscription(string s)
    {
        void OnAudioCreated(AudioClip clip) => SpeakTranscription(clip);
        StartCoroutine(Text2Speech.CreateAudio(s, OnAudioCreated));
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
}