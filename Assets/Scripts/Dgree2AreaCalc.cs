using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dgree2AreaCalc : MonoBehaviour
{
    private static Dgree2AreaCalc _instance;
    public static Dgree2AreaCalc instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Dgree2AreaCalc>();
            return _instance;
        }
    }

    public DgreeAreaPreset data = null;

    public bool forceDefault = false;

    public float _fov{
        get{
            if( data == null )
                return -1;
            
            if( forceDefault ) return data.defaultFOV;

            if( (Application.isPlaying && Camera.main != null) || Camera.main != null ){
                return Camera.main.fieldOfView;
            }else{
                return data.defaultFOV;
            }
        }
    }

    public float _aspect{
        get{
            if( data == null )
                return -1;

            if( forceDefault ) return data.defaultAspect;

            if( (Application.isPlaying && Camera.main != null) || Camera.main != null ){
                return Camera.main.aspect;
            }else{
                return data.defaultAspect;
            }
        }
    }

    // public float _nearPlane{
    //     get{
    //         if( data == null )
    //             return -1;

    //         if( forceDefault ) return data.defaultNearPlane;

    //         if( (Application.isPlaying && Camera.main != null) || Camera.main != null ){
    //             return Camera.main.nearClipPlane;
    //         }else{
    //             return data.defaultNearPlane;
    //         }
    //     }
    // }

    public float nearPlaneArea{
        get{
            // return Mathf.Pow(_nearPlane * Mathf.Tan( (_fov / 2.0f) * Mathf.PI / 180.0f ) * 2.0f, 2) * _aspect;
            return Mathf.Pow(Mathf.Tan( (_fov / 2.0f) * Mathf.PI / 180.0f ) * 2.0f, 2) * _aspect;
        }
    }
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void saved2Dgree(float v){
        if( data.dgreeMapping == null )
            data.dgreeMapping = new List<Vector2>();
        
        data.dgreeMapping.Add(new Vector2(v, getDgree2Area(v)));
        data.dgreeMapping.Sort((v1, v2)=>v1.x.CompareTo(v2.x));
    }

    public void saved2Area(float v){
        if( data.areaMapping == null )
            data.areaMapping = new List<Vector2>();
        
        data.areaMapping.Add(new Vector2(v, getArea2Dgree(v)));
        data.areaMapping.Sort((v1, v2)=>v1.x.CompareTo(v2.x));
    }


    public float getDgree2Area(float dgree){
        // Using Square Area
        return Mathf.Pow(Mathf.Tan( (dgree / 2.0f) * Mathf.PI / 180.0f ) * 2.0f, 2) * 100.0f / nearPlaneArea;
    }

    public float getArea2Dgree(float area){
        return Mathf.Atan( Mathf.Sqrt(area * nearPlaneArea / 100.0f) / 2.0f ) * 180.0f * 2.0f / Mathf.PI;
    }

    public void quickConvert(float v, ref float dgree, ref float area){
        area = getDgree2Area(v);
        dgree = getArea2Dgree(v);
    }

    public void reCalculate(){
        if( data.dgreeMapping != null ){
            for( int i = 0 ; i<data.dgreeMapping.Count ; i++ ){
                data.dgreeMapping[i] = new Vector2(data.dgreeMapping[i].x, getDgree2Area(data.dgreeMapping[i].x));
            }
        }

        if( data.areaMapping != null ){
            for( int i = 0 ; i<data.areaMapping.Count ; i++ ){
                data.areaMapping[i] = new Vector2(data.areaMapping[i].x, getArea2Dgree(data.areaMapping[i].x));
            }
        }
    }
}
