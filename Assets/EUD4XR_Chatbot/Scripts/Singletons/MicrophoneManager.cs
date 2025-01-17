using TMPro;
using UnityEngine;
using ECARules4All_DLL.Utils;
using System.Collections;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    [Header("Debug")]
    public bool listenBack = false;
    private readonly int durationSeconds = 150;

    [Header("Silence Detection")]
    [SerializeField] private float silenceThreshold = 0.02f;
    [SerializeField] private float silenceDuration = 2.5f;
    private float lastSpeechTime;
    
    private float lastAudioLevel;
    private float silenceTimer;
    private bool isSilenceDetectionActive;

    public AudioClip clip;
    private bool isRecording;
    private string currentDevice;
    private const int maxFrequencyHumansHear = 44000;
    public void ChangeMicrophone(string newMic) => currentDevice = newMic;
    public bool IsRecording() => isRecording;

    private int startPosition;
    private System.Action<AudioClip, byte[]> recordingCallback;
    
    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("[WARNING] Micr manager is already recording");
            return;
        }
        
        isRecording = true;
        silenceTimer = 0f;
        isSilenceDetectionActive = false;
        lastSpeechTime = Time.time;
        
        Debug.Log(lastSpeechTime);
        
        #if !UNITY_WEBGL
            Debug.Log("Current Device = " + currentDevice);
            startPosition = Microphone.GetPosition(currentDevice);
            clip = Microphone.Start(currentDevice, false, durationSeconds, maxFrequencyHumansHear);
            
            StartCoroutine(StartSilenceDetection());
        #endif
    }

    private IEnumerator StartSilenceDetection()
    {
        yield return new WaitForSeconds(0.5f);
        isSilenceDetectionActive = true;
        StartCoroutine(MonitorAudioLevel());
    }

    private IEnumerator MonitorAudioLevel()
    {
        while (isRecording && isSilenceDetectionActive)
        {
            float[] samples = new float[128];
            int currentPosition = Microphone.GetPosition(currentDevice);
            clip.GetData(samples, currentPosition - samples.Length);

            float rms = 0f;
            foreach (float sample in samples)
            {
                rms += sample * sample;
            }
            rms = Mathf.Sqrt(rms / samples.Length);
            
            //Debug.Log($"MM1 {rms} - {rms < silenceThreshold}");
            if (rms < silenceThreshold)
            {
                //Debug.Log($"MM2 {lastSpeechTime} - {Time.time - lastSpeechTime} - {Time.time - lastSpeechTime >= silenceDuration}");
                if (Time.time - lastSpeechTime >= silenceDuration)
                {
                    ChatbotManager.Instance.HandleEndRecording();
                    yield break;
                }
            }
            else
            {
                lastSpeechTime = Time.time;
            }

            lastAudioLevel = rms;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void EndRecording(System.Action<AudioClip, byte[]> callback)
    {
        if (!isRecording)
        {
            Debug.LogWarning("[WARNING] Micr manager is not recording");
            return;
        }

        recordingCallback = callback;
        StopAllCoroutines();
        isSilenceDetectionActive = false;

        #if !UNITY_WEBGL
            int endPosition = Microphone.GetPosition(currentDevice);
            Microphone.End(currentDevice);
        #endif

        Debug.LogWarning("START POSITION: " + startPosition);
        Debug.LogWarning("END POSITION: " + endPosition);
        Debug.LogWarning("CHANNELS: " + clip.channels);
        Debug.LogWarning("FREQUENCY: " + clip.frequency);
        Debug.LogWarning("LENGTH: " + clip.length);

        clip = SaveWav.TrimClipDuration(clip, startPosition, endPosition);
        byte[] data = SaveWav.Save(clip);
        
        if (this.listenBack)
        {
            Debug.Log("--------------------Inizio riproduzione");
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
            Debug.Log("--------------------Fine riproduzione");
        }

        isRecording = false;
        callback?.Invoke(clip, data);
    }
    
    [ContextMenu("Play local AudioClip")]
    void DoSomething()
    {
        GetComponent<AudioSource>().Play();
    }
}