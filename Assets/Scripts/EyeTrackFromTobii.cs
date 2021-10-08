using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tobii.XR;
using Tobii.G2OM;

public class EyeTrackFromTobii : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TobiiXR_EyeTrackingData eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        this.transform.position     = eyeTrackingData.GazeRay.Origin;
        this.transform.forward      = eyeTrackingData.GazeRay.Direction;
        this.transform.localScale   = new Vector3(
            (eyeTrackingData.GazeRay.IsValid) ? 1 : 0,
            1, 1
        );
    }
}
