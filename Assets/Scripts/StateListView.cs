using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECARules4All_DLL.Debugger;

public class StateListView : MonoBehaviour
{
    public GameObject prefab;
    public Transform contentParent;

    private string searchQuery = "";
    
    private void OnEnable()
    {
        DataManager.OnSavedDataChanged += HandleDataChanged;
        DataManager.OnActiveStateChanged += HandleActiveStateChanged;
    }
    private void OnDisable()
    {
        DataManager.OnSavedDataChanged -= HandleDataChanged;
        DataManager.OnActiveStateChanged -= HandleActiveStateChanged;
    }

    void Start()
    {
        DebuggerTest.CleanDebuggerFolder();
        DataManager.Initialize();
        DisplayList();
    }

    public void SetSearchQuery(string newText)
    {
        searchQuery = newText.ToLower();
        DisplayList();
    }

    private void HandleDataChanged()
    {
        DisplayList();
    }
    private void DisplayList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        var filteredStates = string.IsNullOrEmpty(searchQuery) ? 
            DataManager.SavedStates :
            DataManager.SavedStates.Where(kvp => kvp.Value.ActionString.ToLower().Contains(searchQuery));

        foreach (var kvp in filteredStates.OrderByDescending(kvp => kvp.Key))
        {
            CreateGameObject(kvp.Value, kvp.Key);
        }
    }
    
    private void HandleActiveStateChanged(int index)
    {
        DisplayList();
    }
    private void CreateGameObject(DebuggerTest.FrozenState s, int index)
    {
        GameObject row = Instantiate(prefab, contentParent);
        row.transform.Find("Index").GetComponent<TextMeshProUGUI>().text = $"{index.ToString()}.";
        row.transform.Find("Info/ActionText").GetComponent<TextMeshProUGUI>().text = s.ActionString;
        row.transform.Find("Info/Date").GetComponent<TextMeshProUGUI>().text = s.Timestamp.ToString();
        
        var activeStateButton = row.transform.Find("Buttons/ActiveStateButton").GetComponent<Button>();
        var ripristinaButton = row.transform.Find("Buttons/RipristinaButton").GetComponent<Button>();
        var canvasGroup = row.GetComponent<CanvasGroup>();
        
        ripristinaButton.onClick.AddListener(() =>
        {
            DataManager.RestoreState(index);
        });
        if (DataManager.ActiveStateIndex < index)
        {
            canvasGroup.alpha = 0.3f;
        }
        if (DataManager.ActiveStateIndex == index)
        {
            ripristinaButton.gameObject.SetActive(false);
            activeStateButton.gameObject.SetActive(true);
        }
    }
}
