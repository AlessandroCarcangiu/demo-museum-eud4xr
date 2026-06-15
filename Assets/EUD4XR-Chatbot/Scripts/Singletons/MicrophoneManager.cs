using System;
using TMPro;
using UnityEngine;
using ECARules4All_DLL.Utils;
using System.Collections;
using ECARules4All_DLL;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    [Header("Main")]
    private AudioSource _audioSource;
    public AudioClip startRecordingSound;
    public AudioClip stopRecordingSound;
    
    
    [Header("Debug")]
    public bool listenBack = false;
    private readonly int durationSeconds = 150;

    [Header("Silence Detection")]
    [SerializeField] private float silenceThreshold = 0.02f;
    [SerializeField] private float silenceDuration = 2.5f;
    private float lastSpeechTime;
    
    // private float lastAudioLevel;
    // private float silenceTimer;
    private bool isSilenceDetectionActive;

    private AudioClip clip;
    private bool isRecording;
    private string currentDevice;
    private const int maxFrequencyHumansHear = 44000;
    public void ChangeMicrophone(string newMic) => currentDevice = newMic;
    public bool IsRecording() => isRecording;

    private int startPosition;
    private System.Action<AudioClip, byte[]> recordingCallback;

    private void Start()
    {
        if (startRecordingSound == null)
            throw new Exception("Start recording sound is not set in ChatbotUIManager");
        
        if (stopRecordingSound == null)
            throw new Exception("Stop recording sound is not set in ChatbotUIManager");
        
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            throw new Exception("AudioSource is not set in ChatbotUIManager");
    }

    public void StartRecording(bool makeBeep = true)
    {
        if (isRecording)
        {
            Debug.LogWarning("[WARNING] Micr manager is already recording");
            return;
        }

        if (makeBeep && !this.listenBack)
        {
            _audioSource.clip = startRecordingSound;
            _audioSource.Play();
        }

        isRecording = true;
        // silenceTimer = 0f;
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

            // lastAudioLevel = rms;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void EndRecording(System.Action<AudioClip, byte[]> callback, bool makeBeep = true)
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
            _audioSource.clip = clip;
            _audioSource.Play();
            Debug.Log("--------------------Fine riproduzione");
        }

        isRecording = false;
        if (makeBeep && !this.listenBack)
        {
            _audioSource.clip = stopRecordingSound;
            _audioSource.Play();
        }
        callback?.Invoke(clip, data);
    }
    
    [ContextMenu("Play local AudioClip")]
    void DoSomething()
    {
        GetComponent<AudioSource>().Play();
    }
}