using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ControllerRecv : MonoBehaviour
{
    public bool triggerSignal{
        set{
            if( _triggerSignal != value ){
                activeBinder.Invoke(value);
                _triggerSignal = value;
            }
        }
    }

    private bool _triggerSignal = false;

    public ActiveBinder activeBinder;

    [Header("Control Controller")]
	public SteamVR_Action_Boolean ControllerTrigger;
	public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        try{
            triggerSignal = ControllerTrigger.GetStateDown(inputSource);
        } catch(System.Exception e){
            Debug.Log(e.Message);
        }


    }

    [System.Serializable]
	public class ActiveBinder : UnityEvent<bool>{}
}
