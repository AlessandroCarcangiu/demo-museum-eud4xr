using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class TTSRequest : MonoBehaviour
{
    private void Start()
    {
        string url = "http://localhost:3000/api/generate-speech";
        string message = "Hello, this is a test message!";
        StartCoroutine(GetSpeechAudioCoroutine(url, message));
    }

    public AudioSource audioSource;

    public IEnumerator GetSpeechAudioCoroutine(string url, string message)
    {
        // Create the JSON payload
        var jsonPayload = new { message = message };
        string jsonString = JsonConvert.SerializeObject(jsonPayload);

        // Create the UnityWebRequest
        UnityWebRequest uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonString);

        // Use UploadHandlerRaw for sending the JSON payload
        uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG); // Expect MP3 data
        uwr.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error in TTS Request: " + uwr.error);
        }
        else
        {
            Debug.Log("TTS Request Successful, Processing Audio...");

            // Directly get the AudioClip from the response
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);

            if (audioSource != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("AudioSource is not set!");
            }
        }
    }
}