using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ObjGen))]
public class ObjGenEditor : Editor
{
    GUIStyle style_foldout = new GUIStyle ();

    public override void OnInspectorGUI(){
        style_foldout = EditorStyles.foldout;
        style_foldout.richText = true;

        // DrawDefaultInspector();

        serializedObject.Update();
        ObjGen objGenScript = (ObjGen)target;

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Global Parameter", EditorStyles.boldLabel);
            if(GUILayout.Button ("Clear Gen. Objs", GUILayout.Width(150))){
                objGenScript.clearSceneObj();
                return;
            }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel += 1;

        var isRandomPos = serializedObject.FindProperty("isRandomPos");
        EditorGUILayout.PropertyField(isRandomPos, new GUIContent("Enable Random Pos.", "Enable Random Pos."), true);

        var roomCenter = serializedObject.FindProperty("roomCenter");
        EditorGUILayout.PropertyField(roomCenter, new GUIContent("Face Center", "Face Center Object"), true);

        EditorGUILayout.BeginHorizontal();
            var SEED = serializedObject.FindProperty("SEED");
            EditorGUILayout.PropertyField(SEED, new GUIContent("Random Seed", "Random Seed"), true);

            if(GUILayout.Button ("New", GUILayout.Width(50))){
                objGenScript.genNewSeed();
                return;
            }
        EditorGUILayout.EndHorizontal();

        var basedPercent = serializedObject.FindProperty("basedPercent");
        EditorGUILayout.PropertyField(basedPercent, new GUIContent("Collider Area Percent", "0 will not effect"), true);

        EditorGUI.indentLevel -= 1;

        EditorGUILayout.Space();

        if( Application.isPlaying ){
            EditorGUILayout.LabelField("Runtime Tools", EditorStyles.boldLabel);
            if(GUILayout.Button ("Gaze All", GUILayout.Width(100))){
                objGenScript.doGazeStuffAll();
                return;
            }
            EditorGUILayout.Space();
        }

        var OnGenerateObjs = serializedObject.FindProperty("OnGenerateObjs");
        EditorGUILayout.PropertyField(OnGenerateObjs, new GUIContent("OnGenerateObjs", "Event Invoke After GenerateObjs"), true);

        EditorGUILayout.Space();
        var genChain = serializedObject.FindProperty("genChain");
        EditorGUILayout.PropertyField(genChain, new GUIContent("genChain", "Generate Chain"), true);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Preset Array", EditorStyles.boldLabel);
        if( objGenScript.objAreaMap != null ){
            if( objGenScript.objAreaMap.Count != 0 ){
                if(GUILayout.Button ("Save Generated Statics")){
                    objGenScript.saveGenStatics();
                }
            }
        }
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button ("Add New Preset")){
            objGenScript.dataPresets.Add(null);
            return;
        }
        if(GUILayout.Button ("Clear", GUILayout.Width(50))){
            objGenScript.dataPresets.Clear();
            return;
        }
        EditorGUILayout.EndHorizontal();

        if( objGenScript.dataPresets != null ){
            var selSetID = serializedObject.FindProperty("selSetID");
            EditorGUILayout.PropertyField(selSetID, new GUIContent("Used Preset ID", "Using ID"), true);

            for( int i=0 ; i<objGenScript.dataPresets.Count ; i++ ){
                var bbSetElement = serializedObject.FindProperty("dataPresets.Array.data["+i.ToString()+"]");
                if(bbSetElement.objectReferenceValue != null){
                    EditorGUILayout.BeginVertical("helpBox");
                    EditorGUI.indentLevel += 1;

                    EditorGUILayout.BeginHorizontal();
                    objGenScript.dataPresets[i].showContent = EditorGUILayout.Foldout(objGenScript.dataPresets[i].showContent, "<b>"+objGenScript.dataPresets[i].name+"</b>", style_foldout);
                    if(GUILayout.Button("Gen", GUILayout.Width(50))){
                        objGenScript.assignDataPresetID(i);
                        if( !Application.isPlaying )
                            objGenScript.nearPlaneArea = Dgree2AreaCalc.instance.nearPlaneArea;
                        objGenScript.generateObject();
                        return; // For Refresh
                    }
                    if(GUILayout.Button("Delete", GUILayout.Width(50))){
                        objGenScript.dataPresets.RemoveAt(i);
                        return; // For Refresh
                    }
                    EditorGUILayout.EndHorizontal();

                    if (objGenScript.dataPresets[i].showContent){
                        if (Selection.activeTransform){
                            EditorGUILayout.PropertyField(bbSetElement, true);
                            EditorGUI.indentLevel += 1;
                            CreateEditor((Study1DataPreset)bbSetElement.objectReferenceValue).OnInspectorGUI();
                            EditorGUI.indentLevel -= 1;

                            EditorGUILayout.BeginVertical("helpBox");
                            EditorGUILayout.LabelField("Total Objects = " + objGenScript.dataPresets[i].objectPerLayer * objGenScript.dataPresets[i].layerYshift.Length);
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                    EditorGUILayout.EndVertical();
                    // EditorGUILayout.Space();
                }else{
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(bbSetElement, true);
                    if(GUILayout.Button("Delete", GUILayout.Width(50))){
                        objGenScript.dataPresets.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        // if(GUILayout.Button("Gen")){
		// 	// tbParseScript.parseStroke();
        //     objGenScript.generateObject();
		// }

        serializedObject.ApplyModifiedProperties();
    }
}
