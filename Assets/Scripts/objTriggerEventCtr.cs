using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class objTriggerEventCtr : MonoBehaviour {
	[TagSelector]
	public string controllerTag;
	// public GameObject getController;
	public KeyCode hotKeyCtr = KeyCode.None;

	[Header("Press Event")]
	public UnityEvent OnTriggerPressed;
	public UnityEvent OnTriggerIn;
	public UnityEvent OnTriggerOut;
	
	// void OnEnable(){
	// 	OnTriggerOut.Invoke();	
	// }

	private void Update() {
		if( hotKeyCtr != KeyCode.None ){
			if( Input.GetKeyDown(hotKeyCtr) ){
				OnTriggerIn.Invoke();
			}
		}
	}

	void OnTriggerEnter(Collider other){
		// if( other.gameObject.GetInstanceID() == getController.GetInstanceID())
		if( other.gameObject.tag == controllerTag )
			OnTriggerIn.Invoke();
	}
	void OnTriggerExit(Collider other){
		if( other.gameObject.tag == controllerTag )
			OnTriggerOut.Invoke();
	}
	void OnTriggerStay(Collider other){
		if( other.gameObject.tag == controllerTag ){
			OnTriggerPressed.Invoke();
			//getController.GetComponent<controllerModelCtr>().triggerCloseOnCollider(0.3f);
		}
	}
}
