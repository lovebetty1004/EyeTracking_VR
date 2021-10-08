using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor.Animations;

#if UNITY_EDITOR
using UnityEditor;
// using UnityEditor.SceneManagement;
#endif

public class StudyRecorder : MonoBehaviour
{
    public string userName = "User001";
    public string studyName = "study1";
    private string studyNameSubfix = "001";
    private string _studyName{
        get{
            return studyName + studyNameSubfix;
        }
    }
    public string savedPath = "";
    private AnimationClip animClip;
    [Space()]
    public ObjGen getStudyObjGen = null;
    // public ADTStudyCtr getADTCtr = null;
    public GameObject studyHelperObj = null;
    public IStudyHelper getStudyHelper = null;
    
    [Space()]
    private ObjGenInfo saveInfo;
    private GameObjectRecorder m_Recorder;
    private bool startRecording = false;

    [Space]
    public GameObject refHeadObj;
    public GameObject refHandLObj;
    public GameObject refHandRObj;

    [Space]
    public bool showScreenGrid = false;
    public RenderTexture screenGrid;
    public Material blitMat;

    private bool showGUI = true;
    private float recordTimeCnt = 0.0f;
    private bool enablePause = false;
    private int pauseCnt = 0;

    void OnValidate (){
        // A Hack for Interface Inspector Field
        if (studyHelperObj != null){
            getStudyHelper = studyHelperObj.GetComponent<IStudyHelper>();
            if ( getStudyHelper == null ) studyHelperObj = null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // if( animClip == null ){
        //     animClip = new AnimationClip();
        // }
        saveInfo                = new ObjGenInfo();
        saveInfo.SEEDS          = new List<int>();
        saveInfo.SEED_Timing    = new List<float>();
        saveInfo.DiseSeeds      = new List<int>();
        saveInfo.remarks        = new List<string>();
        
        m_Recorder = new GameObjectRecorder(this.gameObject);
        m_Recorder.BindComponentsOfType<Transform>(this.gameObject, true);
        m_Recorder.BindComponentsOfType<Collider>(this.gameObject, true);

        // Make File Path & File Name
        makeFileSubfix();
        if( savedPath == "" ){
            createRecordClips(false, Application.dataPath + "/Resources");
        }

        pauseCnt = 0;
    }

    // // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.F1) ){
			showGUI = !showGUI;
		}

        if( showScreenGrid && screenGrid != null && blitMat != null ){
            RenderTexture.active = screenGrid;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenGrid.width, screenGrid.height, 0);
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.Blit(screenGrid, screenGrid, blitMat, 0);
            // Graphics.DrawTexture(new Rect(0, 0, screenGrid.width, screenGrid.height), screenGrid, blitMat, 0);
            GL.PopMatrix();
            RenderTexture.active = null; 
        }
    } 

    void LateUpdate()
    {
        if (animClip == null)
            return;

        if( startRecording ){
            recordTimeCnt += Time.deltaTime;
            m_Recorder.TakeSnapshot(Time.deltaTime);
        }
    }

    public void StartStudy_n_Record(){
        if( getStudyHelper == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign a Study Helper");
        }else{
            getStudyHelper.TriggerStudy(savedPath, _studyName);
        }

        if( animClip == null ){
            animClip = new AnimationClip();
        }

        startRecording = true;
        recordTimeCnt = 0.0f;
    }

    public void PauseStudy_n_Record(){
        startRecording = false;

        if( getStudyHelper == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign ADT Controler");
        }else{
            getStudyHelper.PauseStudy();
        }
    }

    public void ResumeStudy_n_Record(){
        startRecording = true;

        if( getStudyHelper == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign ADT Controler");
        }else{
            getStudyHelper.ResumeStudy();
        }
    }

    public void setEnablePause(bool value){
        enablePause = value;
        pauseCnt++;
    }

    public void SaveSeed(int getSEED){
        saveInfo.SEEDS.Add(getSEED);
        saveInfo.SEED_Timing.Add(recordTimeCnt);
        updateJasonFile();
    }

    public void SaveDise(int getSEED){
        saveInfo.DiseSeeds.Add(getSEED);
        updateJasonFile();
    }

    public void SaveRemarks(){
        saveInfo.remarks.Add(remarkParser());
        updateJasonFile();

        // Clear Data
        PlayerPrefs.SetInt("TriggerL0", 0);
        PlayerPrefs.SetInt("TriggerL1", 0);
        PlayerPrefs.SetInt("TriggerL0_t", 0);
        PlayerPrefs.SetInt("TriggerL1_t", 0);
    }

    public void SaveCommand(string val){
        saveInfo.command += (val + "\n");
        updateJasonFile();
    }

    string remarkParser(){
        string remark = "";
        if( PlayerPrefs.GetInt("TriggerL0_t") != 0 ){
            remark += $"[T0][{(PlayerPrefs.GetInt("TriggerL0")*100.0f/PlayerPrefs.GetInt("TriggerL0_t")).ToString("F1")}%] {PlayerPrefs.GetInt("TriggerL0")}/{PlayerPrefs.GetInt("TriggerL0_t")}";
        }
        if( PlayerPrefs.GetInt("TriggerL1_t") != 0 ){
            remark += $", [T1][{(PlayerPrefs.GetInt("TriggerL1")*100.0f/PlayerPrefs.GetInt("TriggerL1_t")).ToString("F1")}%] {PlayerPrefs.GetInt("TriggerL1")}/{PlayerPrefs.GetInt("TriggerL1_t")}";
            remark += $", [ALL] {(PlayerPrefs.GetInt("TriggerL0")+PlayerPrefs.GetInt("TriggerL1")).ToString()}";
        }
        return remark;
    }

    public void SavePauseInfo(int id, float timeStamp){

    }

    public void SaveResumeInfo(int id, float duration){

    }

    public void SaveRecord(){
        startRecording = false;
        // makeFileSubfix();

        // if( savedPath == "" ){
        //     createRecordClips(false, Application.dataPath + "/Resources");
        // }

        processClipToFile();

        animClip = null;
    }

    public void SaveRecordWithDealy(){
        // Try to avoid animation replay issue with last frame
        StartCoroutine(SaveRecordWithDealy(1.0f));
    }

    IEnumerator SaveRecordWithDealy(float delay){
        yield return new WaitForSeconds(delay);
        SaveRecord();
    }

    void updateJasonFile(){
        if( getStudyObjGen != null ){
            // ObjGenInfo saveInfo = new ObjGenInfo();

            // saveInfo.SEED           = getStudyObjGen.SEED;
            saveInfo.selSetID       = getStudyObjGen.selSetID;
            saveInfo.selSetName     = getStudyObjGen.dataPresets[getStudyObjGen.selSetID].name;
            saveInfo.animClipName   = _studyName;
            saveInfo.applyStudyName = getStudyHelper.getName();

            string saveInfoJasonStr = JsonUtility.ToJson(saveInfo, true);

            try{
                // Debug.Log(saveInfoJasonStr);
                StreamWriter file = new StreamWriter($"{savedPath}/{_studyName}.json");
                file.Write(saveInfoJasonStr);
                file.Close();
                // Debug.Log($"{_studyName}.jason has been Update in: {savedPath}");
            }catch(System.Exception e){
                Debug.Log(e);
            }
        }
    }

    void processClipToFile(){
        if (m_Recorder.isRecording){
            updateJasonFile();
            
            m_Recorder.SaveToClip(animClip);
            AssetDatabase.CreateAsset(animClip, $"{savedPath}/{_studyName}.anim");
            Debug.Log($"{_studyName}.anim has been created in: {savedPath}");
        }
    }

    void makeFileSubfix(){
        studyNameSubfix = "_" + string.Format("{0:D2}_{1:D2} {2:D2}_{3:D2}_{4:D2}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }

    void OnDisable()
    {
        if (animClip == null)
            return;

        // processClipToFile();
        SaveRecord();
    }



    void OnGUI()
    {
        if( showGUI ){
            if( showScreenGrid && screenGrid != null ){
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), screenGrid, ScaleMode.StretchToFill, true, 10.0F);
            }
            
            string recStr = (Mathf.CeilToInt(recordTimeCnt) % 2 == 0) ? "◉" : "◯";
            string pauseStr = (enablePause) ? "\nPause" : "";
            string startStr = (pauseCnt > 0) ? "Resume" : "Start";
            string playStr = (startRecording) ? $"<color=red>{recStr}{pauseStr}</color>" : startStr;
            string timeCntStr = "\n" + getTimeStr(recordTimeCnt);

            Vector2 LBpos = new Vector2(10, Screen.height - 20);

            if( !enablePause && startRecording ){
                GUI.Label(new Rect(LBpos.x, LBpos.y - 100, 100, 100), playStr + "<b>" + timeCntStr + "</b>", GUI.skin.button);
            }else{
                if (GUI.Button(new Rect(LBpos.x, LBpos.y - 100, 100, 100), 
                    playStr + timeCntStr)
                    ){
                    
                    if( startRecording ){
                        PauseStudy_n_Record();
                    }else{
                        if( pauseCnt > 0 ){
                            ResumeStudy_n_Record();
                        }else{
                            StartStudy_n_Record();
                        }
                    }
                    // End of Button if
                }
            }

            GUI.Label(new Rect(LBpos.x + 105, LBpos.y - 100, 50, 20), "Name");
            userName  = GUI.TextField(new Rect(LBpos.x + 155, LBpos.y - 100, 200, 20), userName, 25);
            GUI.Label(new Rect(LBpos.x + 105, LBpos.y - 75, 50, 20), "Study");
            studyName = GUI.TextField(new Rect(LBpos.x + 155, LBpos.y - 75, 200, 20), studyName, 25);
            

            GUI.Label(new Rect(LBpos.x + 105, LBpos.y - 50, 250, 50), $"[Data] <color=cyan>{getStudyObjGen.dataPresets[getStudyObjGen.selSetID].name}</color>\n[S1-2] <color=cyan>{getStudyHelper.getName()}</color>\n{remarkParser()}");
            
            if( recordTimeCnt > 0 ){
                if (GUI.Button(new Rect(LBpos.x, LBpos.y - 130, 100, 25), "<color=blue>Save File</color>") ){
                    SaveRecord();
                }
            }
        }
    }
    void OnDrawGizmos(){
        // Head Part
        if( refHeadObj != null ){
            Gizmos.color = new Color(1, 1, 1, 0.75f);
            Gizmos.matrix = refHeadObj.transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.5f);

            Gizmos.color = new Color(1, 0, 0, 0.75f);
            Gizmos.DrawFrustum(Vector3.zero, 110.0f, 500, 0.3f, 5.0f/3.0f);
        }

        if( refHandLObj != null ){
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.matrix = refHandLObj.transform.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.zero, 0.1f);
        }

        if( refHandRObj != null ){
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.matrix = refHandRObj.transform.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.zero, 0.1f);
        }
        
    }

    public string getTimeStr(float getTime){
		System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(getTime);
		return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
	}

    #if UNITY_EDITOR
    public void createRecordClips(bool customSelect = true, string assignPath = ""){
        // Check Resources File Exist
        if( !Directory.Exists(Application.dataPath + "/Resources") ){
            Directory.CreateDirectory(Application.dataPath + "/Resources");
            Debug.Log("<color=white>[info]</color> Make Dir: " + Application.dataPath + "/Resources");
        }

        string folderPath = assignPath;
        if( customSelect ){
            folderPath = EditorUtility.SaveFolderPanel(
                "Save texture array in a folder",
                Application.dataPath+"/Resources",
                ""
            );

            if (folderPath.Length == 0)
            {
                Debug.LogWarning("Select no folder");
                return;
            }
        }else{
            if( folderPath == "" )
                folderPath = Application.dataPath + "/Resources";
        }
        
        folderPath += "/" + userName;

        if( !Directory.Exists(folderPath) ){
            Directory.CreateDirectory(folderPath);
            Debug.Log("<color=white>[info]</color> Make Dir: "+folderPath);
        }

        var absUri = new Uri(folderPath);
        var assetUri = new Uri(Application.dataPath);
        var relPath = assetUri.MakeRelativeUri(absUri).ToString();
        
        savedPath = relPath;
        // var recordClip = new AnimationClip();

        // AssetDatabase.CreateAsset(recordClip, $"{folderPath}/{clipsName}.anim");
        // Debug.Log($"{clipsName}.anim has been created in: {folderPath}");
    }
    #endif
}

public interface IStudyHelper{
    void TriggerStudy(string getPath = "", string getFileName = "");
    void PauseStudy();
    void ResumeStudy();
    string getName();
}