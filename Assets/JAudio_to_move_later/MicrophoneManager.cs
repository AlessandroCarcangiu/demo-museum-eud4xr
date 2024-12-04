using TMPro;
using UnityEngine;
using ECARules4All_DLL.Utils;


public class MicrophoneManager : Singleton<MicrophoneManager>
{
    private readonly int durationSeconds = 150;

    private AudioClip clip;
    private bool isRecording;
    private string currentDevice;
    private const int maxFrequencyHumansHear = 44000;
    public void ChangeMicrophone(string newMic) => currentDevice = newMic;
    public bool IsRecording() => isRecording;

    private int startPosition;
    
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
            startPosition = Microphone.GetPosition(currentDevice); // Get starting position
            clip = Microphone.Start(currentDevice, false, durationSeconds, maxFrequencyHumansHear);
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
        int endPosition = Microphone.GetPosition(currentDevice); // Get ending position
        Microphone.End(currentDevice); // null is the default device
#endif
        Debug.LogWarning("START POSITION: " + startPosition);
        Debug.LogWarning("END POSITION: " + endPosition);
        Debug.LogWarning("CHANNELS: " + clip.channels);
        Debug.LogWarning("FREQUENCY: " + clip.frequency);
        Debug.LogWarning("LENGTH: " + clip.length);
        clip = SaveWav.TrimClipDuration(clip, startPosition, endPosition);
        // clip = SaveWav.TrimSilence(clip, 0.01f); //TODO Non mi pare funzioni troppo bene
        byte[] data = SaveWav.Save(clip);

        // For Debugging
        // GetComponent<AudioSource>().clip = clip;
        // GetComponent<AudioSource>().Play();
        isRecording = false;
        callback?.Invoke(clip, data);
    }
    
    [ContextMenu("Play local AudioClip")]
    void DoSomething()
    {
        // GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }
}