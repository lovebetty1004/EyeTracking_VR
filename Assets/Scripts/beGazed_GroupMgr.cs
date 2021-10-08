using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beGazed_GroupMgr : MonoBehaviour, IGazeAction
{
    public int idfLabel = 0;
    public SHAPE_TYPE shapeType;
    private List<IGazeAction> childGazeAct;

    public float animationTime = 1.0f;
    public bool continueMode = false;

    // Start is called before the first frame update
    void Start()
    {
        findGazeInterface();
        // for(int i=0 ; i<childGazeAct.Count ; i++ ){
        //     childGazeAct[i].initialParam(animationTime);
        // }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    void findGazeInterface(){
        if( childGazeAct == null ){
            childGazeAct = new List<IGazeAction>();
        }else{
            childGazeAct.Clear();
        }

        Component[] comps = gameObject.GetComponentsInChildren(typeof(IGazeAction));
        // Debug.Log(comps.Length);

        foreach (Component com in comps){
            if( com.gameObject.GetInstanceID() == this.gameObject.GetInstanceID() ){
                continue;
            }
                
            childGazeAct.Add(com as IGazeAction);
        }
    }

    #region IGazeAction Region
    public int identifyLabel(){
        return idfLabel;
    }

    public void initialParam(float timeParam, bool isContinuedMode = false){
        animationTime = timeParam;
        continueMode = isContinuedMode;

        for(int i=0 ; i<childGazeAct.Count ; i++ ){
            childGazeAct[i].initialParam(animationTime);
        }
    }

    public void doGazeStuff(bool isStart = true){
        
    }

    public void adjustCollider(float nearPlaneArea, Vector3 refPos, float basedPercent = 0.0f){
        if( childGazeAct == null || childGazeAct.Count == 0 )
            findGazeInterface();
        
        for( int i=0 ; i<childGazeAct.Count ; i++ ){
            childGazeAct[i].adjustCollider(nearPlaneArea, refPos, basedPercent);
        }
    }

    public void infoCopyer(ref Dictionary<int, Vector3> booker){
        if( childGazeAct == null || childGazeAct.Count == 0 )
            findGazeInterface();
        
        for( int i=0 ; i<childGazeAct.Count ; i++ ){
            childGazeAct[i].infoCopyer(ref booker);
        }
    }

    #endregion
}
