using UnityEngine;

[System.Serializable]
public class HospitalLocation
{
    public string locationName;
    public Transform locationTransform; // Drag empty GameObjects here
    public string situationDescription; // Text shown in UI
    public Color situationColor; // Color for the UI panel
}