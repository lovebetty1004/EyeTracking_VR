using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerMethod_CB : MonoBehaviour, IStudyHelper
{
    [Header("Sequence Setting")]
    public int usedSeq = 0;
    public int nowTask = 0;
    public int trailPerTask = 3;
    public List<int> sequenceID;
    private List<List<int>> seq_permutation;
    [Header("Time Setting")]
    public float roundDuration = 30f;
    public float ISI_Time = 5.0f;
    [Header("Event Control")]
    
    public Event_StudyInfo OnStudyStart;
    public Event_StudyInfo OnStudyStop;

    public UnityEvent[] BeforeRoundStart;
    public UnityEvent OnRoundInitial;
    public UnityEvent OnRoundFinished;
    public UnityEvent OnRoundClear;
    public Event_WordInfo OnWordInfo;

    private Coroutine _round;

    // Start is called before the first frame update
    void Start()
    {
        // var sbs = Combinations<int>.GetCombinations(sequenceID, 6);
        seq_permutation= Combinations<int>.Permutations(sequenceID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Round(){
        for( int i=0 ; i<trailPerTask ; i++ ){
            OnWordInfo.Invoke($"Ready? Round {(i+1).ToString()}", ISI_Time, true);
            BeforeRoundStart[seq_permutation[usedSeq][nowTask]].Invoke();
            yield return new WaitForSeconds(ISI_Time);

            OnRoundInitial.Invoke();

            yield return new WaitForSeconds(roundDuration);

            OnRoundFinished.Invoke();
        }

        OnWordInfo.Invoke("Take a Break~", 10.0f, false);
        OnRoundClear.Invoke();
        nowTask++;
        checkIsLastRound();
    }

    void checkIsLastRound(){
        if( nowTask == seq_permutation[usedSeq].Count ){
            OnStudyStop.Invoke("");
        }
    }

    #region IStudyHelper Implementation
    public void TriggerStudy(string getPath = "", string getFileName = ""){
        if( _round != null)
			StopCoroutine(_round);
        
        if( nowTask == 0 ){
            // First Round
            OnStudyStart.Invoke(getName());
        }
        _round = StartCoroutine(Round());
    }
    public void PauseStudy(){

    }
    public void ResumeStudy(){
        if( _round != null)
			StopCoroutine(_round);
        
        _round = StartCoroutine(Round());
    }
    public string getName(){
        return "["+usedSeq+"] Seq = " + String.Join<int>(", ", seq_permutation[usedSeq]);
    }
    #endregion

    [System.Serializable]
	public class Event_StudyInfo : UnityEvent<string>{}

    [System.Serializable]
	public class Event_WordInfo : UnityEvent<string, float, bool>{}
}
