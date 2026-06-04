using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class CubeLabelTest : MonoBehaviour
{
    [Header("Reference to the TextMeshPro (3D, not UI) component")]
    public TextMeshPro textMeshPro;

    [Header("Text template, use {number} as placeholder")]
    public string textTemplate = "<mark=#C94949E5>{number}</mark>";

    void OnValidate()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponentInChildren<TextMeshPro>();

        if (textMeshPro == null)
            return;

        int cubeNumber = ExtractNumberFromName(gameObject.name) + 1;  // +1 logic from before
        
        int rowNumber = 0;
        if (transform.parent != null)
            rowNumber = ExtractNumberFromName(transform.parent.name);

        int finalNumber = (rowNumber * 10) + cubeNumber;

        string finalText = textTemplate.Replace("{number}", finalNumber.ToString());
        textMeshPro.text = finalText;
    }

    private int ExtractNumberFromName(string objectName)
    {
        Match match = Regex.Match(objectName, @"\((\d+)\)$");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int number)) 
            return number;
        return 0; // -1 means "no number", so +1 in caller gives 0
    }
}