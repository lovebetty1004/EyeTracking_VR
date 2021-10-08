using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable][CreateAssetMenu(fileName = "DgreeAreaPreset", menuName = "DgreeAreaPreset", order = 0)]
public class DgreeAreaPreset : ScriptableObject
{
    public float defaultFOV = 110.1384f;    // Vive Pro Parameter
    public float defaultAspect = 1080.0f/1200.0f; // Vive Pro Resolusion
    // public float defaultNearPlane = 0.3f;
    public List<Vector2> dgreeMapping = new List<Vector2>();
    public List<Vector2> areaMapping = new List<Vector2>();
}
