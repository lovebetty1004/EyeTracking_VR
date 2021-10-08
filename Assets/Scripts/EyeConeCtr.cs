using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EyeConeCtr : MonoBehaviour
{
    [Range(0.1f, 180.0f)]
    public float FOVDgree = 1;

    [Header("Object Setting")]
    public GameObject headObj;
    public GameObject eyeObj;

    [Header("Anchor Setting")]
    public bool enableAnchor;
    public bool enableSmooth = false;
    public float smoothWeight = 1.0f;
    public GameObject anchorObj;

    private Collider coneCollider;

    private bool usingEye = true; // false = usingHead
    private float basedScale = 1000.0f; // Raycast Dest.
    private float dgreeScale = 1.0f;

    public void setUsingEyeOrHead(bool getVal){
        usingEye = getVal;

        if( usingEye && eyeObj != null ){
            this.transform.parent = eyeObj.transform;
        }

        if( !usingEye && headObj != null ){
            this.transform.parent = headObj.transform;
        }

        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = new Vector3(-90, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        if( eyeObj == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign EyeObj.");
        }

        if( headObj == null ){
            Debug.Log("<color=red>[Error]</color> Please Assign headObj.");
        }

        coneCollider = this.gameObject.GetComponent<Collider>();
        
        setUsingEyeOrHead(true);
    }

    public void setNewFOV(float fov){
        FOVDgree = fov;

        dgreeScale = Mathf.Tan(FOVDgree * 0.5f * Mathf.PI / 180.0f) / 0.01745506463f;    // tan(1 dgree);
        Debug.Log(Mathf.Tan(FOVDgree * 0.5f * Mathf.PI / 180.0f));
        this.transform.localScale = new Vector3(basedScale * dgreeScale, basedScale, basedScale * dgreeScale);
    }

    // Update is called once per frame
    void Update()
    {
        coneCollider.enabled = !usingEye || (eyeObj.transform.localScale.x > 0);

        if( enableAnchor != anchorObj.activeSelf ){
            anchorObj.SetActive(enableAnchor);
        }

        if( enableAnchor && eyeObj.transform.localScale.x > 0 ){
            if( enableSmooth ){
                anchorObj.transform.position = Vector3.Lerp(anchorObj.transform.position, this.transform.position - this.transform.up * 10.0f, Time.deltaTime * smoothWeight);
            }else{
                anchorObj.transform.position = this.transform.position - this.transform.up * 10.0f;
            }
            anchorObj.transform.localScale = Vector3.one * 0.1745506463f * 2.0f * dgreeScale * eyeObj.transform.localScale.x;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IGazeAction gazeAction = other.transform.gameObject.GetComponent(typeof(IGazeAction)) as IGazeAction;
        gazeAction.doGazeStuff();

        int ulabel = Mathf.Min(1, gazeAction.identifyLabel());
        PlayerPrefs.SetInt($"TriggerL{ulabel}", PlayerPrefs.GetInt($"TriggerL{ulabel}") + 1);
    }
}
