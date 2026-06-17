using System;
using System.IO;
using ECARules4All_DLL.Utils;
using Pv.Unity;
using UnityEngine;
using Serilog;


public class WakeUpWordHandler : Singleton<WakeUpWordHandler>
{
    public PicovoiceManager picovoiceManager;


    static void inferenceCallback(Inference inference)
    {

    }

    void Start()
    {
        Log.Information("[WakeUpWordHandler - Start] Wakeup Start - started");
        const string FOLDER_NAME = "Picovoice/";
        try
        {
#if UNITY_EDITOR
            var MY_KEYYWORD_PATH = Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}ehi-bot_it_windows_v3_0_0.ppn");
            var MY_RHINOMODEL_PATH =  Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}eud4xr_it_windows_v3_0_0.rhn");
            var APIKEY = Secrets.PICOVOICE_API_KEY_PC;

#else
            var MY_KEYYWORD_PATH = Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}ei-bot_it_android_v3_0_0.ppn");
            var MY_RHINOMODEL_PATH =  Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}ei-bot-android_it_android_v3_0_0.rhn");
            var APIKEY = Secrets.PICOVOICE_API_KEY_HEADSET;
#endif
            Log.Information("[WakeUpWordHandler - Start] befor creating Picovoice manager object");
            picovoiceManager = PicovoiceManager.Create(
                accessKey: APIKEY,
                porcupineModelPath: Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}porcupine_params_it.pv"),
                keywordPath:MY_KEYYWORD_PATH,
                
                wakeWordCallback: ChatbotManager.Instance.HandleStartRecording, //wakeWordCallback,
                contextPath: MY_RHINOMODEL_PATH,
                rhinoModelPath: Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}rhino_params_it.pv"),
                inferenceCallback: inferenceCallback
            );
            Log.Information("[WakeUpWordHandler - Start] Picovoice manager object created");

            picovoiceManager.Start();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        Log.Information("[WakeUpWordHandler - Start] ended");
        // Log.Information("Wakeup Start - started");
        // const string FOLDER_NAME = "Picovoice/";
        // try
        // {
        //     picovoiceManager = PicovoiceManager.Create(
        //         accessKey:"KX4ThQegMktzprqggBJAyDLvQn0xr/tfGtVieePAUMEtYaw/7twfGA==",
        //         keywordPath:Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}ehi-bot_it_windows_v3_0_0.ppn"),
        //         porcupineModelPath: Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}porcupine_params_it.pv"),
        //         wakeWordCallback: ChatbotManager.Instance.HandleStartRecording, // wakeWordCallback,
        //         contextPath: Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}eud4xr_it_windows_v3_0_0.rhn"),
        //         rhinoModelPath: Path.Combine(Application.streamingAssetsPath, $"{FOLDER_NAME}rhino_params_it.pv"),
        //         inferenceCallback: inferenceCallback);
        //     
        //     picovoiceManager.Start();
        // }
        // //
        // //
        // catch(Exception e)
        // {
        //     Debug.LogError(e);
        // }
        //
        // Log.Information("Wakeup Start - ended");
    }
}