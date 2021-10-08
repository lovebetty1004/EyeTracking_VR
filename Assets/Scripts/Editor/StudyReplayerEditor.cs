using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StudyReplayer))]
public class StudyReplayerEditor : Editor
{
    GUIStyle style_foldout = new GUIStyle ();

    public override void OnInspectorGUI(){
        style_foldout = EditorStyles.foldout;
        style_foldout.richText = true;

        DrawDefaultInspector();

        serializedObject.Update();
        StudyReplayer srScript = (StudyReplayer)target;

        EditorGUILayout.Space();
        if(GUILayout.Button("Select User")){
            // srScript.createRecordClips();
            srScript.selectUserData();
            EditorUtility.SetDirty(srScript);
		}

        serializedObject.ApplyModifiedProperties();
    }
}
