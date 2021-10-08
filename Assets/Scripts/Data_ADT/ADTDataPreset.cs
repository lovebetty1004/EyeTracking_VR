using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ADT_StairCase{
    _TwoDownOneUp,
    _TwoUpOneDown
}

public enum ADT_Paradigm{
    _YesNo,
    _3AFC,
    _2AFC,
    _AFC
}

public enum ADT_Calc_Rule{
    _Add,
    _Multiply
}

[System.Serializable][CreateAssetMenu(fileName = "ADTDataPreset", menuName = "ADTDataPreset", order = 0)]
public class ADTDataPreset : ScriptableObject
{
    [HideInInspector]
	public bool showContent = true;
    // public string datasetName = "";
    public ADT_StairCase stairCase = ADT_StairCase._TwoUpOneDown;
    public ADT_Paradigm paradim = ADT_Paradigm._YesNo;
    public ADT_Calc_Rule calcRule = ADT_Calc_Rule._Add;
    public float initialValue = 0.0f;
    public float roundDuration = 30f;
    public float answerDuration = 30f;
    public float ISI_Time = 5.0f;
    public ADTKEY[] ansKeyArr = {ADTKEY.ONE, ADTKEY.TWO, ADTKEY.THREE};
    // public float taskDuration{
    //     get{
    //         return roundDuration + answerDuration;
    //     }
    // }
	public int zeroStopCount = 3;
    public List<float> stepSize;
}
