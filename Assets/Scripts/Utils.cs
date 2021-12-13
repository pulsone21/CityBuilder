using UnityEngine;
using TMPro;

public static class Utils
{

    public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 worldPosition)
    {
        GameObject GO = new GameObject("World_Text", typeof(TextMeshPro));
        Transform transform = GO.transform;
        transform.SetParent(parent);
        transform.localPosition = worldPosition;
        TextMeshPro textMesh = GO.GetComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.text = text;
        textMesh.fontSize = 30;
        textMesh.color = Color.white;
        return textMesh;
    }
}