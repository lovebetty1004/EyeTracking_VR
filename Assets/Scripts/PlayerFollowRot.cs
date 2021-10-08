using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProfCtr{
	public class PlayerFollowRot : MonoBehaviour {
		public bool isFace2cam = true;
		[Space]
		public bool usingBackVec = false;
		public bool useTargetVec = false;
		public bool lockYRot = true;
		[Header("Main Camera GameObject")]
		public GameObject getPlayer;

		[Header("Follow Parameters")]
		public float followSpeed = 1.0f;
		private bool lockFollowing = false;
		private bool camForwardLock = false;
		private Vector3 newFwVec;
		// Use this for initialization
		void Start () {
			if( getPlayer == null ){
				Debug.Log("Does NOT assgin Player, replaced by MainCam");
				getPlayer = Camera.main.gameObject;
			}
			newFwVec = Vector3.Scale(getPlayer.transform.forward, new Vector3(1, 0, 1));
			this.transform.forward = newFwVec.normalized;
		}

		// Update is called once per frame
		void Update () {
			if(lockFollowing || camForwardLock){
				;
			}else{
				if( isFace2cam ){
					// newFwVec = getPlayer.transform.forward;
					newFwVec = getPlayer.transform.position - this.transform.position;
					if( useTargetVec ){
						newFwVec = getPlayer.transform.forward;
					}
					else{
						if( newFwVec.magnitude == 0 )
							newFwVec = getPlayer.transform.forward;
						else
							newFwVec = newFwVec.normalized;
					}
				}
				else{
					// Follow Root Forward
					newFwVec = this.transform.parent.forward;
				}

				if( lockYRot )
					newFwVec.y = 0.0f;
				
				if( usingBackVec )
					newFwVec = -newFwVec;
			}

			this.transform.forward = Vector3.Lerp(this.transform.forward, newFwVec.normalized, followSpeed * Time.deltaTime);
		}
		
		public void face2cam(){
			isFace2cam = true;
		}

		public void face2pivot(){
			isFace2cam = false;
		}

		public void setLock(bool value){
			lockFollowing = value;
		}
		public void setCamLock(bool value){
			camForwardLock = value;
		}
	}
	
}
