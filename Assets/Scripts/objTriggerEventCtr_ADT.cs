using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class objTriggerEventCtr_ADT : MonoBehaviour {
	[TagSelector]
	public string controllerTag;
	// public GameObject getController;
	public KeyCode hotKeyCtr = KeyCode.None;
	public ADTKEY btnKey = ADTKEY.EMPTY;

	[Header("Press Event")]
	public UnityEvent_ADTKEY OnTriggerPressed;
	public UnityEvent_ADTKEY OnTriggerIn;
	public UnityEvent_ADTKEY OnTriggerOut;

	// void OnEnable(){
	// 	OnTriggerOut.Invoke();	
	// }

	private void Update() {
		if( hotKeyCtr != KeyCode.None ){
			if( Input.GetKeyDown(hotKeyCtr) ){
				OnTriggerIn.Invoke(btnKey);
			}
		}
	}

	void OnTriggerEnter(Collider other){
		// Debug.Log(other.gameObject.name);
		// if( other.gameObject.GetInstanceID() == getController.GetInstanceID())
		if( other.gameObject.tag == controllerTag )
			OnTriggerIn.Invoke(btnKey);
	}
	void OnTriggerExit(Collider other){
		if( other.gameObject.tag == controllerTag )
			OnTriggerOut.Invoke(btnKey);
	}
	void OnTriggerStay(Collider other){
		if( other.gameObject.tag == controllerTag ){
			OnTriggerPressed.Invoke(btnKey);
			//getController.GetComponent<controllerModelCtr>().triggerCloseOnCollider(0.3f);
		}
	}

	[System.Serializable]
	public class UnityEvent_ADTKEY : UnityEvent<ADTKEY>{}
}
