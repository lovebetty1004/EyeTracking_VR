// using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjGen : MonoBehaviour
{
    // public int totalObject = 100;
    public List<Study1DataPreset> dataPresets;
    // public List<float> layerDistance;
    // public List<Vector2> sizeRange;
    // public List<GameObject> refObjects;
    public GameObject roomCenter;
    public List<GameObject> savedObjs;
    // Quick Dic
    public Dictionary<int, Vector3> objAreaMap;
    
    public bool isRandomPos = false;
    public int SEED = 306;
    public int selSetID = 0;

    public OnGenerateObjsEvent OnGenerateObjs;

    public List<ObjGen> genChain = new List<ObjGen>();

    [Range(0, 100)]
    public float basedPercent = 0.0f;

    public float nearPlaneArea = 1.0f;

    #region file Writer
    private StreamWriter writer;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        savedObjs = new List<GameObject>();
        objAreaMap = new Dictionary<int, Vector3>();

        // generateObject();
        nearPlaneArea = Dgree2AreaCalc.instance.nearPlaneArea;

        // PlayerPref Counter
        PlayerPrefs.SetInt("TriggerL0", 0);
        PlayerPrefs.SetInt("TriggerL1", 0);
        PlayerPrefs.SetInt("TriggerL0_t", 0);
        PlayerPrefs.SetInt("TriggerL1_t", 0);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public bool assignDataPreset(string getName){
        int getID = dataPresets.FindIndex(item => item.name == getName);

        if( getID < 0 || getID >= dataPresets.Count ){
            Debug.Log("<color=red>[Error]</color> Can not Find: "+getName+", set ID to 0", this);
            selSetID = 0;
            return false;
        }

        selSetID = getID;
        return true;
    }

    public bool assignDataPresetID(int getID){
        if( getID < 0 || getID >= dataPresets.Count){
            Debug.Log("<color=red>[Error]</color> Assign Data Range Error, Set to 0", this);
            selSetID = 0;
            return false;
        }

        selSetID = getID;
        return true;
    }

    public void clearSceneObj(){
        if( savedObjs.Count > 0 ){
            // Remove Objects
            for( int i = 0 ; i<savedObjs.Count ; i++ ){
                DestroyImmediate(savedObjs[i]);
            }
            savedObjs.Clear();
        }

        for( int i=0 ; i<genChain.Count ; i++ ){
            genChain[i].clearSceneObj();
        }
    }

    public void doGazeStuffAll(){
        for( int i=0 ; i<savedObjs.Count ; i++ ){
            IGazeAction gazeAction = savedObjs[i].GetComponent(typeof(IGazeAction)) as IGazeAction;
            gazeAction.doGazeStuff();
        }
    }

    public void genNewSeed(){
        SEED = Random.Range(123, 654321);
    }

    public void assignSeed(int getSeed){
        SEED = getSeed;
    }

    public void setNewBasedPercent(float getPercent){   // 0 ~ 100
        basedPercent = getPercent;
    }

    public void newSeed_n_Generate(){
        genNewSeed();
        generateObject();
    }

    public void generateObject(){
        if( dataPresets == null ){
            Debug.Log("<color=red>[Error]</color> Please Initial DataSet.", this);
            return;
        }

        if( dataPresets.Count == 0 ){
            Debug.Log("<color=red>[Error]</color> Please Initial DataSet.", this);
            return;
        }

        if( objAreaMap == null ){
            objAreaMap = new Dictionary<int, Vector3>();
        }else{
            objAreaMap.Clear();
        }

        clearSceneObj();

        Random.InitState(SEED);

        Debug.Log($"<color=white>[info]</color> Gen: [{selSetID}] {dataPresets[selSetID].name} with SEED = {SEED}");

        Vector3 tmpPos = Vector3.zero;
        int objectperLayer = dataPresets[selSetID].objectPerLayer / dataPresets[selSetID].layerDistance.Count;

        float shiftRadious = Mathf.PI * 2.0f * (1.0f/(float) objectperLayer) * (1.0f / (float)dataPresets[selSetID].layerDistance.Count);

        int L0cnt = 0, L1cnt = 0;
        for( int ly = 0 ; ly < dataPresets[selSetID].layerYshift.Length ; ly++ ){
            for( int i=0 ; i<dataPresets[selSetID].layerDistance.Count ; i++ ){
                for( int x = 0 ; x < objectperLayer ; x++ ){
                    GameObject copyOne = Instantiate(
                        dataPresets[selSetID].refObjects[Random.Range(0, dataPresets[selSetID].refObjects.Count)], 
                        Vector3.zero, Quaternion.identity);
                    IGazeAction gazeAction = copyOne.GetComponent(typeof(IGazeAction)) as IGazeAction;

                    copyOne.transform.parent = this.transform;

                    copyOne.transform.localScale = copyOne.transform.localScale * Random.Range(dataPresets[selSetID].sizeRange[i].x, dataPresets[selSetID].sizeRange[i].y);

                    if( isRandomPos && gazeAction.identifyLabel() == 0 ){
                        tmpPos.x = dataPresets[selSetID].layerDistance[i] * Mathf.Cos(Mathf.PI * 2.0f * (Random.Range(x, x + 0.99f) / (float)objectperLayer) + shiftRadious * i);
                        tmpPos.y = dataPresets[selSetID].layerYshift[ly] + copyOne.transform.localScale.y / 2.0f;
                        tmpPos.z = dataPresets[selSetID].layerDistance[i] * Mathf.Sin(Mathf.PI * 2.0f * (Random.Range(x, x + 0.99f) / (float)objectperLayer) + shiftRadious * i);
                    }else{
                        tmpPos.x = dataPresets[selSetID].layerDistance[i] * Mathf.Cos(Mathf.PI * 2.0f * ((float)x / (float)objectperLayer) + shiftRadious * i);
                        tmpPos.y = dataPresets[selSetID].layerYshift[ly] + copyOne.transform.localScale.y / 2.0f;
                        tmpPos.z = dataPresets[selSetID].layerDistance[i] * Mathf.Sin(Mathf.PI * 2.0f * ((float)x / (float)objectperLayer) + shiftRadious * i);
                    }
                    
                    copyOne.transform.position = tmpPos + this.transform.position;
                    copyOne.transform.LookAt(roomCenter.transform, Vector3.up);
                    
                    // float objArea = gazeAction.findCollidArea() * Mathf.Pow(Camera.main.nearClipPlane / Vector3.Distance(roomCenter.transform.position, copyOne.transform.position + copyOne.transform.forward * copyOne.transform.lossyScale.x * 0.5f), 2);
                    // Debug.Log(objArea / nearPlaneArea, copyOne);
                    gazeAction.adjustCollider(nearPlaneArea, roomCenter.transform.position, basedPercent * 0.01f);
                    gazeAction.initialParam(basedPercent);

                    if( gazeAction.identifyLabel() == 0 ){
                        L0cnt++;
                        copyOne.name = $"[L0][{L0cnt.ToString("000")}] {copyOne.name}";
                    }
                    else{
                        L1cnt++;
                        copyOne.name = $"[L1][{L1cnt.ToString("000")}] {copyOne.name}";
                    }

                    gazeAction.infoCopyer(ref objAreaMap);
                    savedObjs.Add(copyOne);
                }
            }
        }
        
        PlayerPrefs.SetInt("TriggerL0_t", PlayerPrefs.GetInt("TriggerL0_t") + L0cnt);
        PlayerPrefs.SetInt("TriggerL1_t", PlayerPrefs.GetInt("TriggerL1_t") + L1cnt);

        for( int i=0 ; i<genChain.Count ; i++ ){
            genChain[i].assignSeed(SEED+i+1);
            genChain[i].assignDataPresetID(0);
            genChain[i].generateObject();
        }

        OnGenerateObjs.Invoke(SEED);
    }

    public void saveGenStatics(){
        string filePath = $"{Application.streamingAssetsPath}/{dataPresets[selSetID].name}_c{genChain.Count}_info.csv";
        Debug.Log("<color=white>[info]</color> Gen Statics Info CSV File in: "+filePath);

        writer = new StreamWriter(filePath);
        // Basic Information
        writer.WriteLine($"Target Percent,{basedPercent.ToString("F2")}");
        writer.WriteLine("dataPresets,SEEDs,randomPos");
        writer.WriteLine($"{dataPresets[selSetID].name},{SEED},{isRandomPos.ToString()}");
        for( int i=0 ; i<genChain.Count ; i++ ){
            writer.WriteLine($"{genChain[i].dataPresets[genChain[i].selSetID].name},{genChain[i].SEED},{genChain[i].isRandomPos.ToString()}");
        }

        
        Dictionary<float, int> staticRes = new Dictionary<float, int>();
        
        staticParser(ref objAreaMap, ref staticRes);
        for( int i=0 ; i<genChain.Count ; i++ ){
            staticParser(ref genChain[i].objAreaMap, ref staticRes);
        }

        writer.WriteLine("statics results");
        writer.WriteLine("area(%),count");
        List<Vector2> sortStatics = new List<Vector2>();
        foreach (KeyValuePair<float, int> item in staticRes){
            sortStatics.Add(new Vector2(item.Key, item.Value));
        }
        sortStatics.Sort((item1, item2)=>item1.x.CompareTo(item2.x));
        for( int i=0 ; i<sortStatics.Count ; i++ ){
            writer.WriteLine($"{sortStatics[i].x.ToString()},{sortStatics[i].y.ToString("F0")}");
        }

        writer.WriteLine("Raw Data");
        writer.WriteLine("hash,area(%),isTriggered");
        rawDataSaver(ref writer, ref objAreaMap);
        for( int i=0 ; i<genChain.Count ; i++ ){
            rawDataSaver(ref writer, ref genChain[i].objAreaMap);
        }

        writer.Close();
    }

    private void rawDataSaver(ref StreamWriter writer, ref Dictionary<int, Vector3> booker){
        foreach (KeyValuePair<int, Vector3> item in booker){
            writer.WriteLine($"{item.Key},{(item.Value.x*100.0f).ToString("F4")},{item.Value.y}");
        }
    }

    private void staticParser(ref Dictionary<int, Vector3> booker, ref Dictionary<float, int> res){
        float area = 0.0f;

        foreach (KeyValuePair<int, Vector3> item in booker){
            area = item.Value.x * 100.0f;   // Convert to Percent
            
            if( area < 1.0f ){
                area = Mathf.Floor(area * 10.0f)/10.0f;
            }else{
                area = Mathf.Floor(area);
            }

            if( res.ContainsKey(area) ){
                res[area] = res[area] + 1;
            }else{
                res.Add(area, 1);
            }
        }
    }

    [System.Serializable]
	public class OnGenerateObjsEvent : UnityEvent<int>{}
}

public interface IGazeAction{
    int identifyLabel();
    void initialParam(float timeParam, bool isContinuedMode = false);
    void doGazeStuff(bool isStart = true);
    void adjustCollider(float nearPlaneArea, Vector3 refPos, float basedPercent = 0.0f);

    // For Record Something
    void infoCopyer(ref Dictionary<int, Vector3> booker);
    // float findCollidArea();
}

public enum SHAPE_TYPE{
    _Box,
    _Sphere,
    _Capsule,
    _Other
}
