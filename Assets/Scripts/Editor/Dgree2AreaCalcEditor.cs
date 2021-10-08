using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Dgree2AreaCalc))]
public class Dgree2AreaCalcEditor : Editor
{
    GUIStyle style_testArea = new GUIStyle ();
    GUIStyle style_foldout = new GUIStyle ();
    public float inputNum;
    float findDgree = 0;
    float findArea = 0;

    public override void OnInspectorGUI(){
        style_foldout = EditorStyles.foldout;
        style_foldout.richText = true;

        style_testArea = EditorStyles.textArea;
        style_testArea.richText = true;

        // DrawDefaultInspector();
        serializedObject.Update();
        Dgree2AreaCalc d2aScript = (Dgree2AreaCalc)target;

        var data = serializedObject.FindProperty("data");

        if(data.objectReferenceValue != null){
            EditorGUILayout.PropertyField(data, true);
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUI.BeginChangeCheck();
            var forceDefault = serializedObject.FindProperty("forceDefault");
            EditorGUILayout.PropertyField(forceDefault, new GUIContent("", "Froce Default"), true, GUILayout.Width(25));
            if (EditorGUI.EndChangeCheck()){
                d2aScript.reCalculate();
                if( inputNum > 0 ){
                    d2aScript.quickConvert(inputNum, ref findDgree, ref findArea);
                }
            }
            EditorGUILayout.LabelField("Default Param.", EditorStyles.boldLabel);

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel += 1;

            var dataObj = new SerializedObject(d2aScript.data);

            dataObj.Update();
            var defaultFOV = dataObj.FindProperty("defaultFOV");
            EditorGUILayout.PropertyField(defaultFOV, new GUIContent("Fov", "Default if no main camera"), true);

            var defaultAspect = dataObj.FindProperty("defaultAspect");
            EditorGUILayout.PropertyField(defaultAspect, new GUIContent("Aspect", "Default if no main camera"), true);

            // var defaultNearPlane = dataObj.FindProperty("defaultNearPlane");
            // EditorGUILayout.PropertyField(defaultNearPlane, new GUIContent("Near Plane", "Default if no main camera"), true);
            dataObj.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Now: " + ((Camera.main != null)&&!forceDefault.boolValue ? "[MainCam]" : "[Default]"), EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal("HelpBox");
            // EditorGUILayout.TextArea($"Fov = <color=cyan>{d2aScript._fov.ToString("F2")}</color>\nAspect = <color=cyan>{d2aScript._aspect.ToString("F2")}</color>\nNear Plane = <color=cyan>{d2aScript._nearPlane.ToString("F2")}</color>\n", style_testArea);
            string textAreaContent = $"Fov = <color=cyan>{d2aScript._fov.ToString("F2")}</color>\nAspect = <color=cyan>{d2aScript._aspect.ToString("F2")}</color>\n";
            if( (Camera.main != null)&&!forceDefault.boolValue ){
                textAreaContent += $"Width  = <color=cyan>{Camera.main.pixelWidth}</color>\n";
                textAreaContent += $"Height = <color=cyan>{Camera.main.pixelHeight}</color>";
            }
            EditorGUILayout.TextArea(textAreaContent, style_testArea);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Quick View", EditorStyles.boldLabel);

            EditorGUI.indentLevel += 1;
            EditorGUI.BeginChangeCheck();
            inputNum = EditorGUILayout.FloatField("Give a Number:", inputNum);
            if (EditorGUI.EndChangeCheck()){
                EditorGUILayout.BeginHorizontal("HelpBox");
                if( inputNum > 0 ){
                    d2aScript.quickConvert(inputNum, ref findDgree, ref findArea);
                }
            }

            EditorGUILayout.BeginHorizontal("HelpBox");
            if( inputNum > 0 ){
                EditorGUILayout.TextArea($"Dgree = {findDgree.ToString("F2")}°\nArea = {findArea.ToString("F4")}%", style_testArea);
            }else{
                EditorGUILayout.TextArea("Please input > 0.");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 1;

            string listStr = "";
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                if(GUILayout.Button("Save as Area")){
                    d2aScript.saved2Dgree(inputNum);
                }
                listStr = "";
                if( d2aScript.data.dgreeMapping != null ){
                    for( int i=0 ; i<d2aScript.data.dgreeMapping.Count ; i++ ){
                        listStr += $"{d2aScript.data.dgreeMapping[i].x.ToString("00.0")}, <color=orange>{d2aScript.data.dgreeMapping[i].y.ToString("00.0000")}%</color>\n";
                    }
                }else{
                    listStr = "List is Null";
                }
                EditorGUILayout.TextArea(listStr, style_testArea);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if(GUILayout.Button("Save as Dgree")){
                    d2aScript.saved2Area(inputNum);
                }
                listStr = "";
                if( d2aScript.data.areaMapping != null ){
                    for( int i=0 ; i<d2aScript.data.areaMapping.Count ; i++ ){
                        listStr += $"{d2aScript.data.areaMapping[i].x}%, <color=orange>{d2aScript.data.areaMapping[i].y.ToString("F2")}</color>\n";
                    }
                }else{
                    listStr = "List is Null";
                }
                EditorGUILayout.TextArea(listStr, style_testArea);
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }else{
            EditorGUILayout.PropertyField(data, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
