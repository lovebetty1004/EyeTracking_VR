using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ADTDataPreset))]
public class ADTDataPresetEditor : Editor
{
    GUIStyle style_foldout = new GUIStyle ();
    GUIStyle style_textArea = new GUIStyle ();
    string roundInfo = "";
    public override void OnInspectorGUI(){
        style_foldout = EditorStyles.foldout;
        style_foldout.richText = true;

        style_textArea = EditorStyles.textArea;
        style_textArea.richText = true;

        // DrawDefaultInspector();

        serializedObject.Update();
        ADTDataPreset data = (ADTDataPreset)target;
        
        float roundTime = (data.roundDuration + data.ISI_Time) * roundCnt(data.paradim);
        roundInfo = $"Times per Round: <color=orange>{roundTime}</color> sec.";
        // roundInfo += $" |- 1. ISI Time: <color=cyan>{data.ISI_Time}</color>\n";
        // roundInfo += $" |- 2. Round Time: <color=cyan>{data.roundDuration}</color>\n";
        // roundInfo += $" |- 3. Answer";
        // roundInfo += $" |- 3. Answer Time: <color=cyan>{data.answerDuration}</color>";
        EditorGUILayout.TextArea(roundInfo, style_textArea);

        EditorGUILayout.LabelField("Value Setting", EditorStyles.boldLabel);

        EditorGUI.indentLevel += 1;
        var initialValue = serializedObject.FindProperty("initialValue");
        EditorGUILayout.PropertyField(initialValue, true);
        var zeroStopCount = serializedObject.FindProperty("zeroStopCount");
        EditorGUILayout.PropertyField(zeroStopCount, true);
        EditorGUI.indentLevel -= 1;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Study Time Setting", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;

        var ISI_Time = serializedObject.FindProperty("ISI_Time");
        EditorGUILayout.PropertyField(ISI_Time, true);
        var roundDuration = serializedObject.FindProperty("roundDuration");
        EditorGUILayout.PropertyField(roundDuration, true);
        var answerDuration = serializedObject.FindProperty("answerDuration");
        EditorGUILayout.PropertyField(answerDuration, true);

        EditorGUI.indentLevel -= 1;

        EditorGUILayout.LabelField("Study Method", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        
        var calcRule = serializedObject.FindProperty("calcRule");
        EditorGUILayout.PropertyField(calcRule, true);
        var stairCase = serializedObject.FindProperty("stairCase");
        EditorGUILayout.PropertyField(stairCase, true);
        var paradim = serializedObject.FindProperty("paradim");
        EditorGUILayout.PropertyField(paradim, true);

        if( paradim.enumValueIndex == (int)ADT_Paradigm._3AFC || paradim.enumValueIndex == (int)ADT_Paradigm._2AFC ){
            var ansKeyArr = serializedObject.FindProperty("ansKeyArr");
            EditorGUILayout.PropertyField(ansKeyArr, true);
        }

        EditorGUI.indentLevel -= 1;

        EditorGUILayout.BeginHorizontal("HelpBox");
        var stepSize = serializedObject.FindProperty("stepSize");
        EditorGUILayout.PropertyField(stepSize, true);
        EditorGUILayout.EndHorizontal();

        // if(GUILayout.Button("Select Folder")){
		// 	// tbParseScript.parseStroke();
        //     // srScript.createRecordClips();
		// }

        serializedObject.ApplyModifiedProperties();
    }

    private int roundCnt(ADT_Paradigm getParadim){
        switch(getParadim){
            case ADT_Paradigm._YesNo:
                return 1;
            case ADT_Paradigm._3AFC:
                return 3;
            case ADT_Paradigm._2AFC:
                return 2;
            default:
                return 0;
        }
    }
}
