using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ADTStudyCtr))]
public class ADTStudyCtrEditor : Editor
{
    GUIStyle style_foldout = new GUIStyle ();

    public override void OnInspectorGUI(){
        style_foldout = EditorStyles.foldout;
        style_foldout.richText = true;

        // DrawDefaultInspector();

        serializedObject.Update();
        ADTStudyCtr adtScript = (ADTStudyCtr)target;

        // if(GUILayout.Button("Select Folder")){
		// 	// tbParseScript.parseStroke();
		// }

        var ADTData = serializedObject.FindProperty("ADTData");
        
        if(ADTData.objectReferenceValue != null){
            adtScript.ADTData.showContent = EditorGUILayout.Foldout(adtScript.ADTData.showContent, "<b>"+adtScript.ADTData.name+"</b>", style_foldout);

            EditorGUILayout.BeginVertical("HelpBox");
            if( adtScript.ADTData.showContent) {
                if (Selection.activeTransform){
                    EditorGUILayout.PropertyField(ADTData, true);
                    EditorGUI.indentLevel += 1;
                    CreateEditor((ADTDataPreset)ADTData.objectReferenceValue).OnInspectorGUI();

                    if( adtScript.ADTData.paradim == ADT_Paradigm._3AFC || adtScript.ADTData.paradim == ADT_Paradigm._2AFC ){
                        EditorGUILayout.Space();

                        var OnGenDiseSeed = serializedObject.FindProperty("OnGenDiseSeed");
                        EditorGUILayout.PropertyField(OnGenDiseSeed, true);
                        
                    }
                    EditorGUI.indentLevel -= 1;
                }
            }else{
                EditorGUILayout.LabelField("Folding: "+adtScript.ADTData.name, EditorStyles.boldLabel);
            }
            EditorGUILayout.EndVertical();
        }else{
            EditorGUILayout.PropertyField(ADTData, true);
        }

        EditorGUILayout.Space();

        adtScript._foldingHintInpus = EditorGUILayout.Foldout(adtScript._foldingHintInpus, "<b>Hint Input</b>", style_foldout);
        
        if( adtScript._foldingHintInpus ){
            EditorGUI.indentLevel += 1;

            var hapticAction = serializedObject.FindProperty("hapticAction");
            EditorGUILayout.PropertyField(hapticAction, true);

            var inputSource = serializedObject.FindProperty("inputSource");
            EditorGUILayout.PropertyField(inputSource, true);

            EditorGUI.indentLevel -= 1;
        }

        EditorGUILayout.Space();

        adtScript._foldingRoundPart = EditorGUILayout.Foldout(adtScript._foldingRoundPart, "<b>Round Events</b>", style_foldout);

        if( adtScript._foldingRoundPart ){
            var BeforeRoundStart = serializedObject.FindProperty("BeforeRoundStart");
            EditorGUILayout.PropertyField(BeforeRoundStart, true);

            var OnRoundInitial = serializedObject.FindProperty("OnRoundInitial");
            EditorGUILayout.PropertyField(OnRoundInitial, true);

            var OnRoundClear = serializedObject.FindProperty("OnRoundClear");
            EditorGUILayout.PropertyField(OnRoundClear, true);
        }

        EditorGUILayout.Space();

        adtScript._foldingStudyPart = EditorGUILayout.Foldout(adtScript._foldingStudyPart, "<b>Study Events</b>", style_foldout);

        if( adtScript._foldingStudyPart ){
            var OnStudyPause = serializedObject.FindProperty("OnStudyPause");
            EditorGUILayout.PropertyField(OnStudyPause, true);

            var OnStudyResume = serializedObject.FindProperty("OnStudyResume");
            EditorGUILayout.PropertyField(OnStudyResume, true);
            
            var OnStudyStop = serializedObject.FindProperty("OnStudyStop");
            EditorGUILayout.PropertyField(OnStudyStop, true);
        }
        
        EditorGUILayout.Space();

        if( adtScript.answerObjSet == null ){
            EditorGUILayout.HelpBox("Please Fill in: answerObjSet", MessageType.Error);
        }
        var answerObjSet = serializedObject.FindProperty("answerObjSet");
        EditorGUILayout.PropertyField(answerObjSet, true);
        EditorGUI.BeginChangeCheck();
        adtScript.QUES = EditorGUILayout.TextArea(adtScript.QUES);
        if (EditorGUI.EndChangeCheck()){
            EditorUtility.SetDirty(adtScript);
        }

        adtScript._foldingAnswerPart = EditorGUILayout.Foldout(adtScript._foldingAnswerPart, "<b>Answer Events</b>", style_foldout);
        if( adtScript._foldingAnswerPart ){
            var OnAnswerBegin = serializedObject.FindProperty("OnAnswerBegin");
            EditorGUILayout.PropertyField(OnAnswerBegin, true);

            var OnAnswerEnd = serializedObject.FindProperty("OnAnswerEnd");
            EditorGUILayout.PropertyField(OnAnswerEnd, true);
        }

        EditorGUILayout.Space();

        var OnValueChange = serializedObject.FindProperty("OnValueChange");
        EditorGUILayout.PropertyField(OnValueChange, true);

        var OnWordInfo = serializedObject.FindProperty("OnWordInfo");
        EditorGUILayout.PropertyField(OnWordInfo, true);

        // var OnValueUp = serializedObject.FindProperty("OnValueUp");
        // EditorGUILayout.PropertyField(OnValueUp, true);

        // var OnValueDown = serializedObject.FindProperty("OnValueDown");
        // EditorGUILayout.PropertyField(OnValueDown, true);

        serializedObject.ApplyModifiedProperties();
    }
}
