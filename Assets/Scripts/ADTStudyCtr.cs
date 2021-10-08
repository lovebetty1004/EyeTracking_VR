using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Valve.VR;

public enum ADTKEY{
	YES=10, NO=11,
	ONE=1, TWO=2, THREE=3, NONE=-1,
	EMPTY=0
};

public class ADTStudyCtr : MonoBehaviour, IStudyHelper {
	public ADTDataPreset ADTData;

	public GameObject answerObjSet;
	private string folderPath;// = Application.dataPath + "/Resources";
	private StreamWriter writer;
	private int index = 1;

	private float startTimeStamp;
	private float studyTimeCnt = 0.0f;
	private float pauseTimeCnt = 0.0f;
	private bool isStudying = false;
	private bool stackingRound = false;
	private Coroutine _round;
	// public float[] stepSize;
	private float currentValue;
	private int correctCount;
	private int reversalPtCnt;
	private int dice;
	private bool lastAnswer;
	private int _ZeroCount = 0;

	public List<Vector3> reversalPtList;

	public SteamVR_Action_Vibration hapticAction;
	public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
	// public SelectionManager getSM;
	// public GameObject getEXPObjs;
	// public GameObject getAnsSet;
	// public TextMesh getIdxTM;

	public Event_StudyStart BeforeRoundStart;
	public Event_StudyStart OnRoundInitial;
	public Event_StudyStart OnRoundClear;
	public Event_ADT_RoundInfo OnStudyPause;
	public Event_ADT_RoundInfo OnStudyResume;
	public Event_StudyStart OnStudyStop;
	public UnityEvent OnAnswerBegin;
	public UnityEvent OnAnswerEnd;
	// public Event_ADT_ValueChange OnValueUp;
	// public Event_ADT_ValueChange OnValueDown;
	public Event_ADT_ValueChange OnValueChange;
	public Event_ADT_DiseSeed OnGenDiseSeed;

	public Event_WordInfo OnWordInfo;

#region For Editor
	public bool _foldingRoundPart = false;
	public bool _foldingStudyPart = false;
	public bool _foldingAnswerPart = false;
	public bool _foldingHintInpus = false;
#endregion

#region For 3AFC
	private int diseSeed = 306;
	// public ADTKEY[] ans_3AFC = {ADTKEY.ONE, ADTKEY.TWO, ADTKEY.THREE};
	private ADTKEY userSel = ADTKEY.NONE;
	private int trueAns_3AFC = 0;
#endregion

	private void Awake() {
		folderPath = Application.dataPath + "/Resources";
	}
	
	void Start () {
		currentValue = ADTData.initialValue;
		// Debug.Log("Initial delay = "+currentValue);

		_ZeroCount = 0;
		// getAnsSet.SetActive(false);
		lastAnswer = true;
		correctCount = 0;
		reversalPtCnt = 0;
		reversalPtList = new List<Vector3>();

		OnValueChange.Invoke(currentValue);
		OnWordInfo.Invoke("Please Click Start Button to Start.", -1, false);
	}

	void LateUpdate()
	{
		if( isStudying ){
			studyTimeCnt += Time.deltaTime;
		}else{
			if( stackingRound ){
				pauseTimeCnt += Time.deltaTime;
			}
		}
	}

#region IStudyHelper Implementation
	public void TriggerStudy(string getPath = "", string getFileName = ""){
		startStudy(getPath, getFileName);
	}

	public void PauseStudy(){
		isStudying = false;
		stackingRound = true;

		OnStudyPause.Invoke(index, studyTimeCnt);
	}

	public void ResumeStudy(){
		isStudying = true;

		OnStudyResume.Invoke(index, pauseTimeCnt);
		pauseTimeCnt = 0.0f;

		if( stackingRound ){
			DoRound();
			stackingRound = false;
		}
	}

	public string getName(){
		return ADTData.name;
	}

#endregion

	void genNewDise(){
        diseSeed = UnityEngine.Random.Range(123, 765432);
		OnGenDiseSeed.Invoke(diseSeed);
    }

	public void assignDiseSeed(int getSeed){
		diseSeed = getSeed;
	}

	public bool startStudy(string getPath = "", string getFileName = "UserTest"){
		if( getPath != "" ){
			folderPath = getPath;
		}

		if( ADTData == null ){
			Debug.Log("<color=red>[Error]</color> Please Assign ADT Data Preset!");
			return false;
		}else{
			// string filePath = Application.dataPath + folderPath + "/ADT_" + userName + "_" + EXPMode.ToString() + "_" + GetTimeStamp() + ".csv";
			string filePath = $"{getPath}/{getFileName}_ADT.csv";
			Debug.Log("<color=white>[info]</color> ADT CSV File in: "+folderPath);

			writer = new StreamWriter(filePath);

			if( ADTData.paradim == ADT_Paradigm._YesNo ){
				writer.WriteLine("Index,DelayTime,Check,AnswerTime");
			}else{
				writer.WriteLine("Index,DelayTime,Check,Sel,Ans,AnswerTime");
			}

			DoRound();
			
			isStudying = true;
			return true;
		}
	}

	private string GetTimeStamp(){
		var DTN = DateTime.Now;
		return DTN.Month.ToString("D2")+DTN.Day.ToString("D2")+DTN.Hour.ToString("D2")+DTN.Minute.ToString("D2");
	}
	// private void showIndexText(string _stage, Color _color){
	// 	getIdxTM.text = _stage + "階段";
	// 	getIdxTM.color = _color;
	// }
	private void OnDisable() {
		if(writer != null)
			writer.Close();
	}

	public void recvAnswerhandler(ADTKEY recvKey){
		answerObjSet.SetActive(false);
		OnAnswerEnd.Invoke();

		userSel = recvKey;

		if( ADTData.paradim == ADT_Paradigm._YesNo ){
			if( recvKey == ADTKEY.YES ){
				recData(true);
			}else if( recvKey == ADTKEY.NO ){
				recData(false);
			}
		}else if( ADTData.paradim == ADT_Paradigm._3AFC || ADTData.paradim == ADT_Paradigm._2AFC ){
			if( recvKey == ADTData.ansKeyArr[trueAns_3AFC] ){
				recData(true);
			}else{
				recData(false);
			}
		}else{
			// AFC Not Implement
			Debug.Log("<color=red>[!!!!]</color> AFC Not Implement");
		}
	}

	float valueCalc (float v, float step, bool incr){
		switch(ADTData.calcRule){
			case ADT_Calc_Rule._Add:
				return (incr) ? v + step : v - step;
			case ADT_Calc_Rule._Multiply:
				return (incr) ? v / step : v * step;
		}
		return v;
	}

	public void recData(bool _feelTrigger){
		if( _round != null)
			StopCoroutine(_round);

		string checkString = (_feelTrigger)? "Yes":"No";
		string pauseStr = (stackingRound) ? ",Pause" : "";

		// var spendTime = Time.time - startTimeStamp;
		var spendTime = studyTimeCnt - startTimeStamp;

		if( ADTData.paradim == ADT_Paradigm._YesNo ){
			writer.WriteLine($"{(index).ToString()},{currentValue.ToString("F4")},{checkString},{spendTime.ToString("F2")}{pauseStr}");
		}else{
			writer.WriteLine($"{(index).ToString()},{currentValue.ToString("F4")},{checkString},{userSel},{ADTData.ansKeyArr[trueAns_3AFC]},{spendTime.ToString("F2")}{pauseStr}");
		}
		// Debug.Log("<color=blue>Delay Time: "+currentValue+" Get Answer: "+checkString+" ! Spending " + spendTime + " sec.</color>");
		
		// if( (ADTData.stairCase == ADT_StairCase._TwoDownOneUp) ? !_feelTrigger : _feelTrigger ){
		if( _feelTrigger ){
			correctCount++;
			_ZeroCount = 0;
			if(correctCount == 2){ // 2-down
				if(!lastAnswer){ // reversal Point
					reversalPtCnt++;
					reversalPtList.Add(new Vector3(reversalPtCnt, index, currentValue));
					Debug.Log("<color=orange>Reversal Point "+reversalPtCnt+" : Down to Up</color>");
				}

				if( ADTData.stairCase == ADT_StairCase._TwoDownOneUp ){
					currentValue = valueCalc(currentValue, ADTData.stepSize[reversalPtCnt], false);
					// currentValue -= ADTData.stepSize[reversalPtCnt];
					currentValue = (currentValue < 0) ? 0 : currentValue;
				}else{
					currentValue = valueCalc(currentValue, ADTData.stepSize[reversalPtCnt], true);
					// currentValue += ADTData.stepSize[reversalPtCnt];
					currentValue = (currentValue < 0) ? 0 : currentValue;
				}
				
				OnValueChange.Invoke(currentValue);
				correctCount = 0;
				lastAnswer = true;
			}
			else{ // 1-noUP
				// Nothing to do : continue testing
			}
		}
		else{ // 1-up
			if( ADTData.stairCase == ADT_StairCase._TwoDownOneUp ){
				_ZeroCount = (currentValue == ADTData.initialValue)? _ZeroCount+1:0;	
			}else{
				_ZeroCount = (currentValue == 0)? _ZeroCount+1:0;
			}

			if(lastAnswer){ // reversal Point
				reversalPtCnt++;
				reversalPtList.Add(new Vector3(reversalPtCnt, index, currentValue));
				Debug.Log("<color=orange>Reversal Point "+reversalPtCnt+" : Up to Down</color>");
			}

			if( ADTData.stairCase == ADT_StairCase._TwoDownOneUp ){
				currentValue = valueCalc(currentValue, ADTData.stepSize[reversalPtCnt], true);
				// currentValue += ADTData.stepSize[reversalPtCnt];
				currentValue = (currentValue > ADTData.initialValue) ? ADTData.initialValue : currentValue;
			}else{
				currentValue = valueCalc(currentValue, ADTData.stepSize[reversalPtCnt], false);
				// currentValue -= ADTData.stepSize[reversalPtCnt];
				currentValue = (currentValue < 0) ? 0 : currentValue;
			}
			
			OnValueChange.Invoke(currentValue);
			correctCount = 0;
			lastAnswer = false;
		}

		// if(_feelTrigger){
		// 	correctCount++;
		// 	_ZeroCount = 0;
		// 	if(correctCount == 2){ // 2-up
		// 		if(!lastAnswer){ // reversal Point
		// 			reversalPtCnt++;
		// 			reversalPtList.Add(new Vector3(reversalPtCnt, index, currentValue));
		// 			Debug.Log("<color=orange>Reversal Point "+reversalPtCnt+" : Down to Up</color>");
		// 		}
		// 		currentValue += stepSize[reversalPtCnt];
		// 		currentValue = (currentValue < 0)? 0:currentValue;
		// 		correctCount = 0;
		// 		lastAnswer = true;
		// 	}
		// 	else{ // 1-noUP
		// 		// Nothing to do : continue testing
		// 	}
		// }
		// else{ // 1-down
		// 	_ZeroCount = (currentValue == 0)? _ZeroCount+1:0;
		// 	if(lastAnswer){ // reversal Point
		// 		reversalPtCnt++;
		// 		reversalPtList.Add(new Vector3(reversalPtCnt, index, currentValue));
		// 		Debug.Log("<color=orange>Reversal Point "+reversalPtCnt+" : Up to Down</color>");
		// 	}
		// 	currentValue -= stepSize[reversalPtCnt];
		// 	currentValue = (currentValue < 0)? 0:currentValue;
		// 	correctCount = 0;
		// 	lastAnswer = false;
		// }

		index++;
		Debug.Log("Current Value: <color=cyan>"+ currentValue+"</color>");
		// getAnsSet.SetActive(false);

		if(_ZeroCount == ADTData.zeroStopCount){
			Debug.Log("<color=red>Continuous "+ ADTData.zeroStopCount +" times of NO FEELING when no delay, so EXP Stop.</color>");
			writer.WriteLine("ZERO,STILL,NOFEELING");
			writer.WriteLine("NEXT,NEXT,NEXT");
			writer.WriteLine("ReversalPtIndex,ExpIndex,DelayTime");
			for(int i = 0; i < reversalPtList.Count; i++)
				writer.WriteLine(((int)reversalPtList[i].x).ToString()+","+((int)reversalPtList[i].y).ToString()+","+(reversalPtList[i].z).ToString());
			writer.WriteLine("ZERO,STILL,NOFEELING");
			writer.Close();

			answerObjSet.SetActive(false);
			OnStudyStop.Invoke(true);
			OnWordInfo.Invoke("THE END!", -1, false);
			// getIdxTM.text = "THE END!";
			// getIdxTM.color = Color.white;
		}
		else if(reversalPtCnt != ADTData.stepSize.Count-1){
			if( isStudying ){
				DoRound();
			}
		}
		else{
			writer.WriteLine("NEXT,NEXT,NEXT");
			writer.WriteLine("ReversalPtIndex,ExpIndex,DelayTime");
			for(int i = 0; i < reversalPtList.Count; i++)
				writer.WriteLine(((int)reversalPtList[i].x).ToString()+","+((int)reversalPtList[i].y).ToString()+","+(reversalPtList[i].z).ToString());
			writer.Close();
			
			answerObjSet.SetActive(false);
			OnStudyStop.Invoke(true);
			OnWordInfo.Invoke("THE END!", -1, false);
			// getIdxTM.text = "THE END!";
			// getIdxTM.color = Color.white;
		}
	}

	void DoRound(){
		if( ADTData.paradim == ADT_Paradigm._YesNo ){
			_round = StartCoroutine(Round_YN());
		}else{
			_round = StartCoroutine(Round_3AFC());
		}
	}

	public string QUES = "Do you think all the objects in the scene \nare triggered by your eyes?";

	IEnumerator Round_YN(){
		// 1. Start This Round Study
		// startTimeStamp = Time.time;
		answerObjSet.SetActive(false);
		startTimeStamp = studyTimeCnt;

		OnWordInfo.Invoke("Ready?", ADTData.ISI_Time, true);
		BeforeRoundStart.Invoke(true);
		yield return new WaitForSeconds(ADTData.ISI_Time);

		OnRoundInitial.Invoke(true);

		yield return new WaitForSeconds(ADTData.roundDuration);

		OnRoundClear.Invoke(true);
		// 2. User is Answering the Question
		OnWordInfo.Invoke(QUES, ADTData.answerDuration, false);
		answerObjSet.SetActive(true);
		OnAnswerBegin.Invoke();

		try{
			hapticAction.Execute(0, 1, 300, 80, inputSource);
		}
		catch(Exception e){
			// Debug.Log(e.Message);
		}

		yield return new WaitForSeconds(ADTData.answerDuration);
		// 3. Means User DONOT Answer the Question
	}

	IEnumerator Round_3AFC(){
		// 1. Start This Round Study
		// startTimeStamp = Time.time;
		answerObjSet.SetActive(false);
		startTimeStamp = studyTimeCnt;
		genNewDise();

		UnityEngine.Random.InitState(diseSeed);
		trueAns_3AFC = UnityEngine.Random.Range(0, ADTData.ansKeyArr.Length);
		
		Debug.Log($"[Round {index}] Ans = <color=green>{trueAns_3AFC + 1}</color>");

		for( int i=0 ; i<ADTData.ansKeyArr.Length ; i++ ){
			OnWordInfo.Invoke($"Round: {(i+1)}", ADTData.ISI_Time, true);
			BeforeRoundStart.Invoke( i == trueAns_3AFC );
			yield return new WaitForSeconds(ADTData.ISI_Time);

			OnRoundInitial.Invoke( i == trueAns_3AFC );
			yield return new WaitForSeconds(ADTData.roundDuration);
			OnRoundClear.Invoke( i == trueAns_3AFC );
		}

		// 2. User is Answering the Question
		OnWordInfo.Invoke(QUES, ADTData.answerDuration, false);
		answerObjSet.SetActive(true);
		OnAnswerBegin.Invoke();

		try{
			hapticAction.Execute(0, 1, 300, 80, inputSource);
		}
		catch(Exception e){
			// Debug.Log(e.Message);
		}

		yield return new WaitForSeconds(ADTData.answerDuration);
		// 3. Means User DONOT Answer the Question
	}

	[System.Serializable]
	public class Event_StudyStart : UnityEvent<bool>{}

	[System.Serializable]
	public class Event_ADT_DiseSeed : UnityEvent<int>{}

	[System.Serializable]
	public class Event_ADT_ValueChange : UnityEvent<float>{}

	[System.Serializable]
	public class Event_ADT_RoundInfo : UnityEvent<int, float>{}

	[System.Serializable]
	public class Event_WordInfo : UnityEvent<string, float, bool>{}
}
