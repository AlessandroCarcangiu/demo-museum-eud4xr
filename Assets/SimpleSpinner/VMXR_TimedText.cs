using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
/// <summary>
/// Questo script serve a mostrare una lista (non circolare) di testi a schermo. Visualizza subito il primo testo, e poi passa a quello successivo dopo che sono passati waitTime secondi
/// </summary>


/*
 * Scene is loading. Please wait...
 * If you see this canvas for too long, it means that something went wrong. Try reloading the application. If problem persists, please check console log for more details.
 */
public class VMXR_TimedText : MonoBehaviour
{
    public List<string> textList; // Va inizializzata dall'editor!!!
    public TextMeshProUGUI tmpGUI;
    public float secondsToWait;

    private Stopwatch watch;

    private int textCount = 0;

    // Start is called before the first frame update
    private void Start()
    {
        watch = Stopwatch.StartNew();
        secondsToWait *= 1000; // Li trasformo in millisecondi

        if (textList.Count > 1)
        {
            tmpGUI.text = textList[0];
        }
        else
        {
            textCount = -1; // Così evita di entrare nel secondo if dell'Update nel caso in cui la lista sia vuota
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (watch.ElapsedMilliseconds > secondsToWait) // Controllo se è passato abbastanza tempo
        {
            if (textCount < textList.Count - 1) // Controllo se ci sono ancora testi da mostrare
            {
                tmpGUI.text = textList[++textCount];
                watch = Stopwatch.StartNew();
            }
        }
    }
}
