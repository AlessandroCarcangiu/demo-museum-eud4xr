using UnityEngine;
using System;
using System.Reflection;

[DefaultExecutionOrder(-200)] // Forza questo script a partire PRIMA di qualsiasi DLL
public class ECACrashDetector : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("<color=orange><b>[DETECTOR] Inizio simulazione preventiva della DLL...</b></color>");

        // 1. Intercettiamo la classe RuleUtils dentro la DLL del tutor
        Type ruleUtilsType = null;
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            ruleUtilsType = asm.GetType("ECARules4All_DLL.Utils.RuleUtils");
            if (ruleUtilsType != null) break;
        }

        if (ruleUtilsType == null)
        {
            Debug.LogError("[DETECTOR] Errore: Impossibile trovare la classe 'RuleUtils' nella DLL!");
            return;
        }

        // 2. Becchiamo l'esatta funzione che va in NullReference
        MethodInfo targetMethod = ruleUtilsType.GetMethod("RetrieveECAAttributes", 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        if (targetMethod == null)
        {
            Debug.LogError("[DETECTOR] Errore: Impossibile trovare il metodo 'RetrieveECAAttributes' nella DLL!");
            return;
        }

        // 3. Troviamo tutti gli oggetti nella scena
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int analizzati = 0;

        foreach (GameObject go in allObjects)
        {
            if (go == null) continue;

            // Filtriamo solo gli oggetti che hanno script del framework per andare veloci
            Component[] components = go.GetComponents<Component>();
            bool isEcaObject = false;
            foreach (var c in components)
            {
                if (c != null && (c.GetType().Name.Contains("ECA") || c.GetType().Name.Contains("EUD4XR")))
                {
                    isEcaObject = true;
                    break;
                }
            }

            if (!isEcaObject) continue;
            analizzati++;

            // 4. Simuliamo l'esecuzione della DLL dentro una gabbia di sicurezza (try-catch)
            try
            {
                object instance = targetMethod.IsStatic ? null : Activator.CreateInstance(ruleUtilsType);
                targetMethod.Invoke(instance, new object[] { go });
            }
            catch (TargetInvocationException e)
            {
                // SE FA CRASH: Abbiamo trovato il colpevole!
                if (e.InnerException is NullReferenceException)
                {
                    string path = GetGameObjectPath(go);
                    Debug.LogError($"<color=red><b>🚨 [KILLER TROVATO!] 🚨</b></color>");
                    Debug.LogError($"L'oggetto che rompe la DLL è: <color=yellow><b>{go.name}</b></color>");
                    Debug.LogError($"Percorso esatto nella Hierarchy: <color=cyan>{path}</color>");
                    Debug.LogError($"Componente specifico: Controlla questo GameObject nell'Inspector!");
                    break; // Ci fermiamo al primo che rompe la catena
                }
            }
            catch (Exception)
            {
                // Ignoriamo altri errori di setup della reflection
            }
        }

        Debug.Log($"<color=orange><b>[DETECTOR] Simulazione completata. Analizzati {analizzati} oggetti ECA.</b></color>");
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }
}