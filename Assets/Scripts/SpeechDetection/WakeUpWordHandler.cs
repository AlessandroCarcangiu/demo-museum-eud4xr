using System;
using System.Collections.Generic;
using System.IO;
using ECARules4All_DLL.Utils;
using Pv.Unity;
using UnityEngine;


/*public class WakeUpWordHandlerAR : MonoBehaviour
{
    private static System.Timers.Timer _timer;

    private SpeechRecognizer recognizer;

    private float lastSpeechTime;
    private float silenceTime = 3f; 
    private KeywordRecognitionModel keywordModel;

    private bool firstRegistration = false;

    [Obsolete]
    async void Start()
    {
        await DefineModelKeyword();
        if (firstRegistration == false)
        {
            await recognizer.StartKeywordRecognitionAsync(keywordModel);
            firstRegistration = true;
        }
    }
    
    [Obsolete]
    public async Task DefineModelKeyword()
    {
        string keywordModelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "6c48c38c-35a1-4514-9e2c-213b7ae35f2f.table");
        
        var config = SpeechConfig.FromSubscription(keywordModelPath, "region");
        
        config.SetProperty("SpeechServiceConnection_RecoModelPath", "path_to_model_directory");
        config.SetProperty("SpeechServiceConnection_OfflineMode", "true");
        
        config.SpeechRecognitionLanguage = "it-IT";
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput(); //
        if (recognizer != null)
        {
            Debug.Log("Arresto del riconoscitore precedente.");
            await recognizer.StopContinuousRecognitionAsync();
            recognizer.Dispose();
            recognizer = null;
        }
        recognizer = new SpeechRecognizer(config, audioConfig);
        
        
        Debug.Log("Keyword model path: " + keywordModelPath);
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("is android");

            using (var www = UnityEngine.Networking.UnityWebRequest.Get(keywordModelPath))
            {
                www.SendWebRequest(); // await www.SendWebRequest();

                if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load keyword model: " + www.error);
                    return;
                }
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "bot.table"), www.downloadHandler.data);
                keywordModelPath = Path.Combine(Application.persistentDataPath, "bot.table");
            }
        }

        if (!System.IO.File.Exists(keywordModelPath))
        {
            Debug.LogError("Keyword model file not found at: " + keywordModelPath);
            return;
        }

        keywordModel = KeywordRecognitionModel.FromFile(keywordModelPath);

        if (keywordModel != null)
        {

            Debug.Log("modello caricato");
            recognizer.Recognized += OnRecognized;
            recognizer.Canceled += OnCanceled;
            recognizer.Recognizing += (s, e) =>
            {
                Debug.Log($"Recognizing modelloKEYWORD: {e.Result.Text}");
            
                lastSpeechTime = Time.time; // Inizio
            
                //STOPPARE LA REGISTRAZIONE VIA VOCE
            
                string recognizedText = e.Result.Text;
            
                string StopWord = "stop";
            
                string FermatiWord = "fermati";
            
                if (recognizedText.Contains(StopWord, StringComparison.OrdinalIgnoreCase))
                {
            
                    Debug.Log($"La parola chiave '{StopWord}' è stata trovata!");
                    //StopConversation(); // todo RIMETTERE
                }
            
                if (recognizedText.Contains(FermatiWord, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"La parola chiave '{FermatiWord}' è stata trovata!");
                    //StopConversation(); // todo RIMETTERE
                }
            };
            Debug.Log("eventi associati");
        }
        else
        {
            Debug.Log("modello non caricato");
        }
    }

    [Obsolete]
    private void OnRecognized(object sender, SpeechRecognitionEventArgs e)
    {
        Debug.Log("dentro onRecognized");
        if (e.Result.Reason == ResultReason.RecognizedKeyword)
        {
            Debug.Log("Wake-up word detected!");
            //StartContinuousRecognition();
            GameObject button = GameObject.Find("RegistraButton");
            if (button == null)
            {
                Debug.Log("bottone registra non trovato");
            }
            else
            {
                //StartRecognition(button); // todo RIMETTERE
    
                //richiamata solo per il primo messaggio (qui si cambia solo il bottone)
    
                Debug.Log("chiamata start recording avvenuta");
    
                lastSpeechTime = Time.time; //Inizio
            }
        }
    
        else if (e.Result.Reason == ResultReason.RecognizedSpeech)
        {
            Debug.Log($"Speech recognized: {e.Result.Text}");
            //transcribedText = e.Result.Text;  // todo RIMETTERE
            lastSpeechTime = Time.time; //ultima parola
        }
        else
        {
            Debug.Log("Recognized event but no keyword detected.");
        }
    }
    
    private void OnCanceled(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        Debug.LogError($"Recognition canceled: {e.Reason}. Error details: {e.ErrorDetails}");
    }
}*/


public class WakeUpWordHandler : Singleton<WakeUpWordHandler>
{
    public PicovoiceManager picovoiceManager;
    
    static void wakeWordCallback()
    {
        Debug.Log("Wake-up word detected!");
    }

    static void inferenceCallback(Inference inference)
    {
        /*Debug.Log("start inferenceCallback");
        
        if(inference.IsUnderstood)
        {
            string intent = inference.Intent;
            Dictionary<string, string> slots = inference.Slots;
            // take action based on inferred intent and slot values
            Debug.Log("ho triggerato la key");
        }
        else
        {
            Debug.Log("qua ho detto cose a caso, forse");
        }
        
        Debug.Log("end inferenceCallback");*/
    }

    void Start()
    {
        Debug.Log("Wake Up started 1");

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

        Debug.Log("Wakeup started 2");
    }
    
    /*void Update()
    {
        if (!picovoiceManager.IsRecording)
        {
            if (picovoiceManager.IsAudioDeviceAvailable())
            {
                picovoiceManager.Start();
            }
            else
                Debug.LogError("No audio recording device available!");
        }
    }*/
} 