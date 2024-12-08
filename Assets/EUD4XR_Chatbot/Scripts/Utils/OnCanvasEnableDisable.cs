using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class OnCanvasEnableDisable<T> : MonoBehaviour where T : MonoBehaviour
{
    private Canvas canvas;
    
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in " + gameObject.name);
        }
    }

    
    public void DoEnable(T newDataSource)
    {
        if (newDataSource != null) dataSourceRef = newDataSource;
        canvas.enabled = true;
        UpdateTexts();
    }
    
    public void DoDisable()
    {
        canvas.enabled = false;
    }


    public T dataSourceRef;

    public void UpdateTarget(T newDataSource)
    {
        dataSourceRef = newDataSource;
        UpdateTexts();
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

    private void UpdateTexts()
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

public class OnCanvasEnableDisable : OnCanvasEnableDisable<MonoBehaviour>
{
}