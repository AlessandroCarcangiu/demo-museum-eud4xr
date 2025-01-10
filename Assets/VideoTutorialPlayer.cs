using System;
using System.Collections;
using System.Linq;
using ECARules4All_DLL.Utils;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class VideoTutorialPlayer : MonoBehaviour
{
    private void Start()
    {
        Start_PositionRelated();
        Start_VideoSource();
    }

    private void OnEnable()
    {
        Debug.LogError("ONENABLE");
    }

    private GameObject RecursiveFindChild(GameObject parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        foreach (Transform child in parent.transform)
        {
            var result = RecursiveFindChild(child.gameObject, childName);
            if (result != null)
                return result;
        }

        return null;
    }

    #region PositionRelated

    [Header("Position Related")] public InputActionReference toggleVisibilityInputActionReference;
    public GameObject objectToToggleVisibility;

    private double _lastTime;

    private void ToggleVisibility(InputAction.CallbackContext callbackContext)
    {
        if (objectToToggleVisibility != null)
        {
            var newStatus = !(objectToToggleVisibility.gameObject.activeSelf);

            if (newStatus == false)
            {
                _lastTime = _videoPlayer.time;
                objectToToggleVisibility.SetActive(false);
            }
            else
            {
                objectToToggleVisibility.SetActive(true);
                Start_VideoSource();
            }
        }
    }

    private void Start_PositionRelated()
    {
        toggleVisibilityInputActionReference.action.performed += ToggleVisibility;
    }

    #endregion

    #region VideoRelated

    [Header("Video Related")] [SerializeField]
    private GameObject screen;

    private void Start_VideoSource()
    {
        Debug.LogError("START VIDEO SOURCE");
        this.TrySetCanvas("PlaneVideo");
        // SelectVideo(source); // Non deve essere fatto allo start perché altrimenti darebbe errore non appena viene aggiunto il componente

        volume = volume > maxVolume ? maxVolume : volume;
        volume = volume < 0.0f ? 0.0f : volume;
        ChangesVolume(volume);

        SelectVideo(source);

       
        if (stopped) this.Stops();
        else if (paused) this.Pauses();
        else this.Plays();

        if (paused || playing)
            ChangesCurrentTime(_lastTime);
    }


    private void TrySetCanvas(string canvasName)
    {
        if (screen == null || screen.name != canvasName)
        {
            var checkCanvas = RecursiveFindChild(gameObject, canvasName);

            if (checkCanvas == null) // Se è null vuol dire che non esiste un figlio con quel nome
            {
                // Nota: Find("") restituisce il gameobject padre (quello a cui appartiene il transform)
                screen = gameObject;
            }
            else
            {
                screen = checkCanvas.gameObject;
            }
        }

        _videoPlayer = screen.GetComponent<VideoPlayer>();

        if (_videoPlayer == null)
            _videoPlayer = screen.AddComponent<VideoPlayer>();

        _videoPlayer.playOnAwake = false;
        _videoPlayer.isLooping = true;
    }

    public void SelectVideo(string videoName)
    {
        if (string.IsNullOrEmpty(videoName))
        {
            Debug.LogWarning("Hai inserito un url vuoto per il video!");
            return;
        }

        var oldName = _videoPlayer.url.Split('/').Last(); // Recupero il vecchio nome del file
        if (string.Equals(oldName, videoName))
            return; // Se il file non è cambiato esco dalla funzione

        if (_videoPlayer.isPlaying)
            Stops();

        _videoPlayer.url = TaxonomyUtils.getFileVideoByName(videoName);
        source = videoName;
        // duration = _videoPlayer.length;
    }

    #region ECA

    public string source;

    public float volume;
    public float maxVolume = 1.0f;

    // public double duration;
    // public double currentTime;

    public ECABoolean playing = new ECABoolean(ECABoolean.BoolType.NO);
    public ECABoolean paused = new ECABoolean(ECABoolean.BoolType.NO);
    public ECABoolean stopped = new ECABoolean(ECABoolean.BoolType.YES);

    private VideoPlayer _videoPlayer;

    /// <summary>
    /// <b>Plays</b> starts the video.
    /// </summary>
    public void Plays()
    {
        this.playing = new ECABoolean(ECABoolean.BoolType.YES);
        this.stopped = new ECABoolean(ECABoolean.BoolType.NO);
        this.paused = new ECABoolean(ECABoolean.BoolType.NO);
        _videoPlayer.Play();
    }

    /// <summary>
    /// <b>Pauses</b> pauses the video.
    /// </summary>
    public void Pauses()
    {
        this.playing = new ECABoolean(ECABoolean.BoolType.NO);
        this.stopped = new ECABoolean(ECABoolean.BoolType.NO);
        this.paused = new ECABoolean(ECABoolean.BoolType.YES);
        _videoPlayer.Pause();
    }

    /// <summary>
    /// <b>Stops</b> stops the video.
    /// </summary>
    public void Stops()
    {
        this.playing = new ECABoolean(ECABoolean.BoolType.NO);
        this.stopped = new ECABoolean(ECABoolean.BoolType.YES);
        this.paused = new ECABoolean(ECABoolean.BoolType.NO);
        // this.currentTime = 0.0;

        _videoPlayer.Stop();
    }

    /// <summary>
    /// <b>ChangesVolume</b> changes the video volume to the given value.
    /// If the value is greater than the max volume, the volume is set to the max volume.
    /// If the value is lower than 0, the volume is set to 0.
    /// </summary>
    /// <param name="v">The new video volume. </param>
    public void ChangesVolume(float v)
    {
        if (v > maxVolume)
        {
            v = maxVolume;
        }

        if (v < 0)
        {
            v = 0;
        }

        volume = v;
        _videoPlayer.SetDirectAudioVolume(0, volume);
        //trackindex is set to 0, but there may be more than 1 audio track
    }

    /// <summary>
    /// <b>ChangesSource</b> changes the video source to the given value.
    /// The new path must be relative to the user-accessible Inventory folder.
    /// </summary>
    /// <param name="newSource">The path for the new video file.</param>
    public void ChangesSource(string newSource)
    {
        SelectVideo(newSource);
    }

    private void ChangesCurrentTime(double c)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeCurrentTime(c));
    }
    
    private IEnumerator ChangeCurrentTime(double c)
    {
        float maxWaitTime = 5.0f;
        while (_videoPlayer.length == 0 && maxWaitTime > 0)
        {
            yield return null;
            maxWaitTime -= Time.deltaTime;
        }

        if (maxWaitTime <= 0)
        {
            throw new TimeoutException("Timeout while waiting for the video to load.");
        }

        if (c <= _videoPlayer.length)
        {
            var frameRate = _videoPlayer.frameRate;
            var seek = (frameRate * c);
            _videoPlayer.frame = (long)(seek);
        }
    }

    public void DecreaseCurrentTimeBy(float decrease)
    {
        var currentTime = _videoPlayer.time;
        var c = currentTime - decrease;
        ChangesCurrentTime(c);
    }

    public void IncreaseCurrentTimeBy(float increase)
    {
        var currentTime = _videoPlayer.time;
        var c = currentTime + increase;
        ChangesCurrentTime(c);
    }

    #endregion

    #endregion
}