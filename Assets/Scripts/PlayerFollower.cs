using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProfCtr{
	public class PlayerFollower : MonoBehaviour {
		[Header("Main Camera GameObject")]
		public GameObject getPlayer;
		[Header("Camera Lock Setting")]
		public PlayerFollowRot getPFR;
		public LayerMask getLM;
		[TagSelector]
		public string[] hitObjectsTag;

		[Header("Follow Parameters")]
		public float followSpeed = 1.0f;
		public Vector3 followedAxis = new Vector3(1, 0, 1);
		[Space]
		public float yawSpeed = 1.0f;
		public Vector2 yawShifter = Vector2.zero;
		public GameObject yawTarget = null;
		// public float yawScaler = 1.0f;
		private Plane virtualHoriz;
		public bool lockFollowing = false;

		private Vector3 horizFw, horizUp;
		private float savedY = 0;
		// private bool camForwardLock = false;
		// Use this for initialization
		void Start () {
			if( getPlayer == null ){
				Debug.Log("Does NOT assgin Player, replaced by MainCam");
				getPlayer = Camera.main.gameObject;
			}

			if( yawShifter.magnitude != 0 ){
				followedAxis.y = 0;
			}

			this.transform.position = Vector3.Scale(getPlayer.transform.position, followedAxis)
									+ Vector3.Scale(this.transform.position, Vector3.one - followedAxis);

			if( yawTarget != null ){
				horizFw = yawTarget.transform.position - getPlayer.transform.position;
				horizUp = Vector3.Cross(horizFw.normalized, getPlayer.transform.right);

				virtualHoriz = new Plane(horizUp, yawTarget.transform.position);
				savedY = getPlayer.transform.position.y + yawTarget.transform.localPosition.y;;
			}
		}

		// Update is called once per frame
		void Update () {
			setCamLock(false);
			RaycastHit hit;
			if(Physics.Raycast(getPlayer.transform.position, getPlayer.transform.forward, out hit, 3f, getLM)){
				for(int i = 0; i < hitObjectsTag.Length; i++){
					if(hit.collider.tag == hitObjectsTag[i]){
						Debug.Log("hit lock~!");
						setCamLock(true);
						// return;
					}
				}
			}
			// else{
			// 	setCamLock(false);
			// }

			// if(lockFollowing || camForwardLock){
			// 	return;
			// }
			if(lockFollowing)
				return;
			
			float yawShiftValue = 0;
			if( yawTarget != null && yawShifter.magnitude != 0 ){
				savedY = getPlayer.transform.position.y + yawTarget.transform.localPosition.y;
				
				Vector3 tmpPos = yawTarget.transform.position;
				tmpPos.y = savedY;

				horizFw = tmpPos - getPlayer.transform.position;
				horizUp = Vector3.Cross(horizFw.normalized, getPlayer.transform.right);

				virtualHoriz.SetNormalAndPosition(horizUp, tmpPos);
				Debug.DrawLine(tmpPos, tmpPos + horizUp);
				yawShiftValue = virtualHoriz.GetDistanceToPoint(getPlayer.transform.position + getPlayer.transform.forward * horizFw.magnitude);
				yawShiftValue = (yawShiftValue == 0) ? 0 : Mathf.Sign(yawShiftValue) * yawSpeed;
			}

			Vector3 newFollowPos = new Vector3(getPlayer.transform.position.x * followedAxis.x, 
											getPlayer.transform.position.y * followedAxis.y,
											getPlayer.transform.position.z * followedAxis.z);

			Vector3 followOriPos = new Vector3(this.transform.position.x * (1.0f - followedAxis.x), 
											this.transform.position.y * (1.0f -  followedAxis.y) + yawShiftValue,
											this.transform.position.z * (1.0f -  followedAxis.z));
			newFollowPos = newFollowPos + followOriPos;

			if( yawTarget != null && yawShifter.magnitude != 0 ){
				newFollowPos.y = Mathf.Min(Mathf.Max(followOriPos.y, savedY + yawShifter.x), savedY + yawShifter.y);
			}
			
			this.transform.position = Vector3.Lerp(this.transform.position, newFollowPos, followSpeed * Time.deltaTime);
		}

		private void setCamLock(bool value){
			// this.camForwardLock = value;
			if( getPFR == null )
				return;
			getPFR.setCamLock(value);
		}

		public void setLock(bool value){
			lockFollowing = value;
		}

		public void lockYFollow(){
			followedAxis.y = 0;
		}

		public void unlockYFollow(){
			followedAxis.y = 1;
		}

		public void setFollowSpeed(float _speed){
			followSpeed = _speed;
		}
	}

}
