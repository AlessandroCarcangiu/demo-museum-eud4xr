using TMPro;
using UnityEngine;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    private readonly string fileName = "output.wav";
    private readonly int duration = 5;

    private AudioClip clip;
    private bool isRecording;
    private string currentDevice;

    public void ChangeMicrophone(string newMic) => currentDevice = newMic;
    public bool IsRecording() => isRecording;

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("[WARNING] Micr manager is already recording");
            return;
        }
        
        isRecording = true;
        #if !UNITY_WEBGL
            Debug.Log("Current Device = " + currentDevice);
            clip = Microphone.Start(currentDevice, false, duration, 44100);
        #endif
    }

    public void EndRecording(System.Action<AudioClip, byte[]> callback)
    {
        if (!isRecording)
        {
            Debug.LogWarning("[WARNING] Micr manager is not recording");
            return;
        }

#if !UNITY_WEBGL
        Microphone.End(currentDevice); // null is the default device
#endif

        byte[] data = SaveWav.Save(fileName, clip);
        // SaveWav.TrimSilence(clip, 0.01f); //TODO Non mi pare funzioni troppo bene

        isRecording = false;
        GetComponent<AudioSource>().clip = clip;
        callback?.Invoke(clip, data);
    }
}