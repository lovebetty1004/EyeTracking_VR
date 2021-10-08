using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTwin : MonoBehaviour
{
    public GameObject refObj;
    // Start is called before the first frame update
    void Start()
    {
        if( refObj == null )
            Debug.Log("<color=red>[Error]</color> Please Assign Twin Target OBject!");
    }

    // Update is called once per frame
    void Update()
    {
        if( refObj == null )
            return;
        this.transform.position = refObj.transform.position;
        this.transform.rotation = refObj.transform.rotation;
        this.transform.localScale = refObj.transform.localScale;
    }
}
