using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class MicrophoneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Button recordButton;
    // [SerializeField] private Image progressBar;
    public TextMeshProUGUI message;
    public TMP_Dropdown dropdown;

    private readonly string fileName = "output.wav";
    private readonly int duration = 5;

    public AudioSource speaker;
    private AudioClip clip;
    private bool isRecording;
    // private float time;
    // private OpenAIApi openai = new OpenAIApi();

    private void Start()
    {
        dropdown.ClearOptions();
#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        recordButton.onClick.AddListener(StartRecording);
        // recordButton.clicked += StartRecording;
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
#endif
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording()
    {
        isRecording = true;
        recordButton.enabled = false;
        var index = PlayerPrefs.GetInt("user-mic-device-index");

#if !UNITY_WEBGL
        clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
#endif
    }

    private async void EndRecording()
    {
        message.text = "Transcripting...";

#if !UNITY_WEBGL
        Microphone.End(null);
#endif

        byte[] data = SaveWav.Save(fileName, clip);
        // SaveWav.TrimSilence(clip, 0.01f); //TODO Non mi pare funzioni troppo bene

        speaker.clip = clip; // Per debugging. Posso ascoltare la clip creata. Teoricamente si può togliere questa riga di codice
        
        // do a POST Unity web request to localhost:5000/transcribe-audio
        string url = "http://localhost:3000/api/audio";
        void UpdateCanvasWithTranscription(string res)
        {
            message.text = res;
            recordButton.enabled = true;
        }
        StartCoroutine(GetTranscriptionGivenAudioClip(url, data, UpdateCanvasWithTranscription));
    }
    IEnumerator GetTranscriptionGivenAudioClip(string url, byte[] byteArray, System.Action<string> callback)
    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, "POST");
        
        // Set up the request with a byte array of raw data
        UploadHandler uploader = new UploadHandlerRaw(byteArray);
        uploader.contentType = "application/octet-stream"; // Set content type for raw binary data
        uwr.uploadHandler = uploader;
        
        // Set up the download handler to capture the response text
        DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
        uwr.downloadHandler = downloadHandler;
        
        yield return uwr.SendWebRequest();
        
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            throw new Exception("Error: " + uwr.error);
        }
        // Log the response text (the transcription result)
        string responseText = downloadHandler.text;
        Debug.Log("Response Text: " + responseText);

        // Optional: Parse the JSON response if needed (assuming it's in JSON format)
        try
        {
            // Example of parsing a JSON response
            var jsonResponse = JsonConvert.DeserializeObject<AudioResponse>(responseText);
            if (jsonResponse != null && jsonResponse.success)
            {
                Debug.Log("Transcription: " + jsonResponse.transcription);
                callback?.Invoke(jsonResponse.transcription);
            }
            else
            {
                throw new Exception("Transcription not found or failed.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error parsing JSON response: " + ex.Message);
        }
    }

    // Define a response structure to match your server's JSON response
    [System.Serializable]
    public class AudioResponse
    {
        public bool success;
        public string transcription;
    }
    
    private void Update()
    {
        if (isRecording)
        {
            // time += Time.deltaTime;
            // progressBar.fillAmount = time / duration;
            
            // If I press the space bar, it will stop recording
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isRecording = false;
                EndRecording();
            }
            // if (time >= duration)
            // {
            //     time = 0;
            //     isRecording = false;
            //     EndRecording();
            // }
        }
    }
}