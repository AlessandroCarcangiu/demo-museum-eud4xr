using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQuitApplication : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
