using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StudyRecorder))]
public class StudyRecorderEditor : Editor
{
    GUIStyle style_foldout = new GUIStyle ();

    public override void OnInspectorGUI(){
        style_foldout = EditorStyles.foldout;
        style_foldout.richText = true;

        DrawDefaultInspector();

        serializedObject.Update();
        StudyRecorder srScript = (StudyRecorder)target;

        if(GUILayout.Button("Select Folder")){
			// tbParseScript.parseStroke();
            srScript.createRecordClips();
		}

        serializedObject.ApplyModifiedProperties();
    }
}
