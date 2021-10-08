using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable][CreateAssetMenu(fileName = "Study1DataPreset", menuName = "Study1DataPreset", order = 0)]
public class Study1DataPreset : ScriptableObject
{
    [HideInInspector]
	public bool showContent = true;
    // public string datasetName = "";
    public int objectPerLayer = 100;
    public float[] layerYshift = {0};
    public List<float> layerDistance;
    public List<Vector2> sizeRange;
    public List<GameObject> refObjects;
}
