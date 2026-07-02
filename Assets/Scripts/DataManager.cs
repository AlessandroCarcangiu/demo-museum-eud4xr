using System;
using System.IO;
using System.Collections.Generic;
using ECARules4All_DLL.Debugger;
using UnityEngine;

public static class DataManager 
{
    public static Dictionary<int, DebuggerTest.FrozenState> SavedStates { get; private set; }
    public static int ActiveStateIndex { get; private set; } = 0;
    
    public static event System.Action OnSavedDataChanged;
    public static event System.Action<int> OnActiveStateChanged; 

    public static void Initialize()
    {
        SavedStates = new Dictionary<int, DebuggerTest.FrozenState>();
        ReadSavedData();

        //DebuggerTest.OnStateSaved -= HandleNewSave;
        //DebuggerTest.OnStateSaved += HandleNewSave;
    }

    private static void ReadSavedData()
    {
        var index = 0;

        while (true)
        {
            try
            {
                SavedStates.Add(index, DebuggerTest.ReadJsonFile(index));
                index += 1;
            }
            catch (FileNotFoundException)
            {
                break;
            }
        }
    }

    private static void HandleNewSave()
    {
        ReloadSavedData();

        ActiveStateIndex = SavedStates.Count;
        
        OnSavedDataChanged?.Invoke();
        OnActiveStateChanged?.Invoke(ActiveStateIndex);
    }

    public static void RestoreState(int index)
    {
        DebuggerTest.RestoreStateAtIndex(index);
        ActiveStateIndex = index;
        
        OnActiveStateChanged?.Invoke(ActiveStateIndex);
    }

    private static void ReloadSavedData()
    {
        ClearSavedData();
        ReadSavedData();
    }
    
    private static void ClearSavedData()
    {
        SavedStates.Clear();
    }
}