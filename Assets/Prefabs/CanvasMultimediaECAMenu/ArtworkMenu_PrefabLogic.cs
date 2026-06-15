using System;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class ArtworkMenu_PrefabLogic : MonoBehaviour
{
    [Serializable]
    public class ListItem
    {
        public GameObject obj;
        public TMP_Text label;
    }
    
    public ListItem videoItem;
    public ListItem audioItem;
    public ListItem textItem;
    public ListItem lightsItem;

    private void Awake()
    {
        if (videoItem == null) throw new Exception("The video text must be assigned");
        if (audioItem == null) throw new Exception("The audio text must be assigned");
        if (textItem == null) throw new Exception("The text text must be assigned");
        if (lightsItem == null) throw new Exception("The lights text must be assigned");
    }

    private void Start()
    {
        OnObjectLoaded?.Invoke();
    }
    
    // Create a Unity public event called OnObjectLoaded
    // This event will be triggered when the object is loaded
    public event Action OnObjectLoaded;

    public void LoadArguments(ArtworkMenu_AttachToECAObject owner)
    {
        var c = owner.videoList.Count;
        videoItem.label.text = $"{c} video";
        if (c == 0) videoItem.obj.SetActive(false);
        
        c = owner.audioList.Count;
        audioItem.label.text = $"{c} audio";
        if (c == 0) audioItem.obj.SetActive(false);
        
        c = owner.textList.Count;
        textItem.label.text = $"{c} descrizioni testuali";
        if (c == 0) textItem.obj.SetActive(false);
        
        c = owner.lightList.Count;
        lightsItem.label.text = $"{c} luci";
        if (c == 0) lightsItem.obj.SetActive(false);
    }
    
}
