using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTrackTest : MonoBehaviour
{
    public GameObject eyeObj;
    public LayerMask hitLM;
    RaycastHit hit;

    [Header("Anchor Setting")]
    public bool enableAnchor;
    public bool enableSmooth = false;
    public float smoothWeight = 1.0f;
    public GameObject anchorObj;

    private bool usingEye = true; // false = usingHead

    public void setUsingEyeOrHead(bool getVal){
        usingEye = getVal;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( eyeObj == null ){
            return;
        }

        bool isValid = (usingEye) ? (eyeObj.transform.localScale.x > 0) : true;
        Vector3 basePos = (usingEye) ? eyeObj.transform.position : this.transform.position;
        Vector3 baseFw  = (usingEye) ? eyeObj.transform.forward : this.transform.forward;

        if( eyeObj.transform.localScale.x > 0 ){    // isValid
            if (Physics.Raycast(basePos, baseFw, out hit, Mathf.Infinity, hitLM)){
                IGazeAction gazeAction = hit.transform.gameObject.GetComponent(typeof(IGazeAction)) as IGazeAction;

                gazeAction.doGazeStuff();

                int ulabel = Mathf.Min(1, gazeAction.identifyLabel());
                PlayerPrefs.SetInt($"TriggerL{ulabel}", PlayerPrefs.GetInt($"TriggerL{ulabel}") + 1);

                Debug.DrawRay(basePos, baseFw*1000, Color.black);
            }else{
                Debug.DrawRay(basePos, baseFw*1000, Color.white);
            }
        }else{
            Debug.DrawRay(basePos, baseFw, Color.gray);
        }

        if( enableAnchor != anchorObj.activeSelf ){
            anchorObj.SetActive(enableAnchor);
        }

        if( enableAnchor && eyeObj.transform.localScale.x > 0 ){
            if( enableSmooth ){
                anchorObj.transform.position = Vector3.Lerp(anchorObj.transform.position, basePos + baseFw.normalized * 10.0f, Time.deltaTime * smoothWeight);
            }else{
                anchorObj.transform.position = basePos + baseFw.normalized * 10.0f;
            }
            anchorObj.transform.localScale = Vector3.one * eyeObj.transform.localScale.x;
        }

    }
}
