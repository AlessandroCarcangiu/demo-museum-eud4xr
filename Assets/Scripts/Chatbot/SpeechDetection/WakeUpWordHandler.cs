using System;
using System.Collections.Generic;
using System.IO;
using ECARules4All_DLL.Utils;
using Pv.Unity;
using UnityEngine;
using Serilog;


public class WakeUpWordHandler : Singleton<WakeUpWordHandler>
{
    public PicovoiceManager picovoiceManager;
    
    static void wakeWordCallback()
    {
        Debug.Log("Wake-up word detected!");
    }

    static void inferenceCallback(Inference inference)
    {

    }

    void Start()
    {
        Log.Information("Wakeup Start - started");
        
        try
        {
            picovoiceManager = PicovoiceManager.Create(
                accessKey:"QbodFcLg52O1IfQdo5G6UQaqqb3rB3+QdBBtJr/7Gw5Xumz4I9lqVw==",
                keywordPath:Path.Combine(Application.streamingAssetsPath, "ehi-bot_it_windows_v3_0_0.ppn"),
                porcupineModelPath: Path.Combine(Application.streamingAssetsPath, "porcupine_params_it.pv"),
                wakeWordCallback: ChatbotManager.Instance.HandleStartRecording, // wakeWordCallback,
                contextPath: Path.Combine(Application.streamingAssetsPath, "eud4xr_it_windows_v3_0_0.rhn"),
                rhinoModelPath: Path.Combine(Application.streamingAssetsPath, "rhino_params_it.pv"),
                inferenceCallback: inferenceCallback);
            
            picovoiceManager.Start();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }

        Log.Information("Wakeup Start - ended");
    }
} 