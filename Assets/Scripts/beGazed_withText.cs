using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class beGazed_withText : beGazed, IGazeAction
{
    private MeshRenderer mRender;
    public TextMesh showPercent;

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public new void adjustCollider(float nearPlaneArea, Vector3 refPos, float basedPercent = 0.0f){
        base.adjustCollider(nearPlaneArea, refPos, basedPercent);

        if( showPercent != null ){
            showPercent.text = (areaPercent*100.0f).ToString("F4") + "%";
        }
    }
    
}
