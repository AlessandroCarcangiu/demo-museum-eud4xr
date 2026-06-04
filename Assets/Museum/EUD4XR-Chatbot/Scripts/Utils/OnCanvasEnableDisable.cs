using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class OnCanvasEnableDisable<T> : MonoBehaviour where T : MonoBehaviour
{
    public GameObject iGameObjectProviderRef;
    private ISourceProvider<T> iSourceProviderRef;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            throw new Exception("Canvas not found in " + gameObject.name);
        }

        iSourceProviderRef = iGameObjectProviderRef.GetComponent<ISourceProvider<T>>();
        
        if (iSourceProviderRef == null)
        {
            throw new Exception("iSourceProviderRef not found in " + gameObject.name);
        }
    }


    public void OnEnable()
    {
        canvas.enabled = true;
        UpdateUI();
    }

    public void OnDisable()
    {
        canvas.enabled = false;
    }


    private void UpdateUI()
    {
        var cached = iSourceProviderRef.dataSourceRef;
        if (cached == null) return;
        UpdateTexts(cached);
    }

    #region UpdateTextOnEnable

    [Serializable]
    public class Data2String : SerializableCallback<T, string>
    {
    }

    [Serializable]
    public class TextItem
    {
        public TMP_Text textRef;
        public Data2String onCanvasEnableCallback;
    }

    private void UpdateTexts(T dataSourceRef)
    {
        foreach (var textItem in textItems)
        {
            textItem.textRef.text = textItem.onCanvasEnableCallback.Invoke(dataSourceRef);
        }
    }

    #endregion

    [Header("Update Texts on Canvas Enable")]
    public List<TextItem> textItems;
}

// public abstract class OnCanvasEnableDisable : OnCanvasEnableDisable<MonoBehaviour>
// {
// }