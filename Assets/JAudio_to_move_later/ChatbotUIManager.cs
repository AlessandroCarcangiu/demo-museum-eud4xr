using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatbotUIManager : Singleton<ChatbotUIManager>
{
    public TextMeshProUGUI message;
    public TMP_Dropdown d_micList;

    public AudioSource speaker;

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

        // void OnButtonClick()
        // {
        //     if (MicrophoneManager.Instance.IsRecording())
        //     {
        //         MicrophoneManager.Instance.EndRecording();
        //         return;
        //     }
        //     MicrophoneManager.Instance.StartRecording(); // StartRecording();
        // }
        // b_speakToChatbot.onClick.AddListener(OnButtonClick);
            
        void OnDropdownValueChanged(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
            string newMic = Microphone.devices[index];
            MicrophoneManager.Instance.ChangeMicrophone(newMic);
        }
        d_micList.onValueChanged.AddListener(OnDropdownValueChanged);

        var index = PlayerPrefs.GetInt("user-mic-device-index", 0);
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
        void OnAudioCreated(AudioClip clip)
        {
            speaker.clip = clip;
            speaker.Play();
        }
        StartCoroutine(Text2Speech.CreateAudio(s, OnAudioCreated));
    }
    
    public void SpeakTranscription(AudioClip clip)
    {
        speaker.clip = clip;
        speaker.Play();
        
    }
}