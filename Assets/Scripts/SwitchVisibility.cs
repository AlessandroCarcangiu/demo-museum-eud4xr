using UnityEngine;

public class SwitchVisibility : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToToggle;

    public void ToggleVisibility()
    {
        foreach (var obj in objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf);
            }
            else
            {
                Debug.LogWarning("Encountered a null GameObject in the objectsToToggle list.");
            }
        }
    }
}
