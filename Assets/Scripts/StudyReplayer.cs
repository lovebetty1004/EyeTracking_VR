// Method From https://forum.unity.com/threads/how-can-i-add-an-animation-to-a-timeline-via-script.1063355/

using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Assertions;
using UnityEditor.Animations;

#if UNITY_EDITOR
using UnityEditor;
// using UnityEditor.SceneManagement;
#endif

public class StudyReplayer : MonoBehaviour
{
    public PlayableDirector replayer;
    public Animator twinAnimator;
    [Space]
    public ObjGen getStudyObjGen = null;
    public ADTStudyCtr getADTCtr = null;

    [HideInInspector]
    public ObjGenInfo saveInfo;
    [HideInInspector]
    public AnimationClip saveClip;

    [Space]
    public GameObject refHeadObj;
    public GameObject refHandLObj;
    public GameObject refHandRObj;

    [Space]
    public bool showScreenGrid = false;
    public RenderTexture screenGrid;
    public Material blitMat;

    private bool showGUI = true;
    private bool isReplaying = false;
    // private bool shouldInit = true;

    private int genSeedID = 0;
    private int diseSeedID = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

    void addClip2Timeline(){
        if( replayer == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign PlayableDirector.");
            return;
        }

        if( twinAnimator == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign Animator.");
            return;
        }

        TimelineAsset asset = replayer.playableAsset as TimelineAsset;
        
        foreach (TrackAsset track in asset.GetOutputTracks())
            if (track.name == "USER_DATA_TRACK")
                asset.DeleteTrack(track);
        
        AnimationTrack newTrack = asset.CreateTrack<AnimationTrack>("USER_DATA_TRACK");

        // 2. Make the created animation track reference the animationToAdd
        TimelineClip clip = newTrack.CreateClip(saveClip);
        clip.start = 0.5f;
        clip.timeScale = 1f;
        clip.duration = clip.duration / clip.timeScale;
 
        // 3. Edit the director's TimelineInstance and configure the bindings to reference objectToAnimate
        replayer.SetGenericBinding(newTrack, twinAnimator);

        replayer.time = 0;
        replayer.Evaluate();
    }

    void OnGUI()
    {
        if( showGUI ){
            if( showScreenGrid && screenGrid != null ){
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), screenGrid, ScaleMode.StretchToFill, true, 10.0F);
            }

            string playStr = (isReplaying) ? "<color=blue>Pause</color>" : "Play";
            string timeCntStr = "\n" + getTimeStr(replayer.time) + " | " + getTimeStr(replayer.duration);

            Vector2 LBpos = new Vector2(10, Screen.height - 20);

            if (GUI.Button(new Rect(LBpos.x, LBpos.y - 100, 100, 100), 
				playStr + timeCntStr)
                ){
                
                if( isReplaying ){
                    PauseHandler();
                }else{
                    PlayHandler();
                }
			}
            if( GUI.Button(new Rect(LBpos.x + 100 + 5, LBpos.y - 100 + 30, 25, 70), "RE") ){
                RestartHandler();
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

    public void queryNextGenSeed(){
        if( getStudyObjGen == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign ObjGen.");
            return;
        }

        getStudyObjGen.assignSeed(saveInfo.SEEDS[genSeedID]);
        genSeedID = (genSeedID + 1) % saveInfo.SEEDS.Count;
    }

    public void queryNextDiseSeed(){
        if( getADTCtr == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign ADTCtr.");
            return;
        }

        if( saveInfo.DiseSeeds == null || saveInfo.DiseSeeds.Count == 0 ){
            Debug.Log("<color=red>[Error]</color> No Dice Data.");
        }else{
            getADTCtr.assignDiseSeed(saveInfo.DiseSeeds[diseSeedID]);
            diseSeedID = (diseSeedID + 1) % saveInfo.DiseSeeds.Count;
        }
        
    }

    void PlayHandler(){
        if( getStudyObjGen == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign ObjGen.");
            return;
        }

        // getStudyObjGen.assignDataPresetID(saveInfo.selSetID);
        getStudyObjGen.assignDataPreset(saveInfo.selSetName);
        getStudyObjGen.assignSeed(saveInfo.SEEDS[0]);

        if( getADTCtr == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign ADTCtr.");
            return;
        }
        
        if( saveInfo.DiseSeeds == null || saveInfo.DiseSeeds.Count == 0 ){
            Debug.Log("<color=red>[Error]</color> No Dice Data.");
        }else{
            getADTCtr.assignDiseSeed(saveInfo.DiseSeeds[0]);
        }

        isReplaying = true;
        getADTCtr.startStudy(Application.streamingAssetsPath, saveInfo.animClipName+"_replay");
        replayer.Play();
    }

    void RestartHandler(){
        genSeedID = 0;
        diseSeedID = 0;

        // shouldInit = true;

        replayer.Stop();
        replayer.time = 0;
        replayer.Evaluate();

        PlayHandler();
    }

    void PauseHandler(){
        // isReplaying = false;
        // replayer.Pause();
    }

    public string getTimeStr(double getTime){
		System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(getTime);
		return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
	}


#if UNITY_EDITOR
    public void selectUserData(){
        string userJasonFilePath = "";

        userJasonFilePath = EditorUtility.OpenFilePanel(
            "Find User Jason File", 
            Application.dataPath + "/Resources", "json");
        
        if (userJasonFilePath.Length == 0){
            Debug.LogWarning("Select no file");
            return;
        }else{
            string jsonContent = File.ReadAllText(userJasonFilePath);

            Debug.Log("<color=white>[Info]</color> Load: "+userJasonFilePath);
            saveInfo = JsonUtility.FromJson<ObjGenInfo>(jsonContent);
            
            // Parse Animation Clip
            try{
                int assetsIndex = userJasonFilePath.IndexOf("Asset");
                int lastSlashIndex = userJasonFilePath.LastIndexOf('/');

                string clipPath = userJasonFilePath.Substring(
                    assetsIndex, Mathf.Max(1, lastSlashIndex - assetsIndex)) + 
                    "/" + saveInfo.animClipName + ".anim";
                // string clipPath = userJasonFilePath.Substring(
                //     0, Mathf.Max(1, lastSlashIndex)) + 
                //     "/" + saveInfo.animClipName + ".anim";

                saveClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimationClip));   // NOT work in Folder StreamAssets

                Debug.Log("<color=white>[Info]</color> Load: " + clipPath + "\nStudy Duration = " + saveClip.averageDuration);

                addClip2Timeline();

            } catch(Exception e){
                Debug.Log(e);
            }

            string[] results;
            if( !getStudyObjGen.assignDataPreset(saveInfo.selSetName) ){
                results = AssetDatabase.FindAssets(saveInfo.selSetName);
                foreach (string guid in results){
                    Study1DataPreset tmpData = (Study1DataPreset) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Study1DataPreset));
                    getStudyObjGen.dataPresets.Add(tmpData);
                    getStudyObjGen.assignDataPreset(saveInfo.selSetName);
                    Debug.Log("<color=white>[Info]</color> Try to Load: " + AssetDatabase.GUIDToAssetPath(guid));
                    break;
                }
            }

            if( saveInfo.applyStudyName != "" && saveInfo.applyStudyName != getADTCtr.ADTData.name ){
                results = AssetDatabase.FindAssets(saveInfo.applyStudyName);
                foreach (string guid in results){
                    ADTDataPreset tmpData = (ADTDataPreset) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ADTDataPreset));
                    getADTCtr.ADTData = tmpData;
                    Debug.Log("<color=white>[Info]</color> Try to Load: " + AssetDatabase.GUIDToAssetPath(guid));
                    break;
                }
            }
        }
    }
#endif
}
