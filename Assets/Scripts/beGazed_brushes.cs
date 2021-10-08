using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class beGazed_brushes : beGazed, IGazeAction
{
    // public SHAPE_TYPE shapeType;
    private bool startAnimate = false;
    public float animTime = 1.0f;
    private float animTimeCnt = 0.0f;
    public AnimationCurve _animCurve = AnimationCurve.Linear(0, 0, 1, 1);
    // Start is called before the first frame update

    // [Space]
    // private Collider coli;
	[SerializeField] Renderer[] _linkedRenderers;
    MaterialPropertyBlock _sheet;

    void Start()
    {
        // _linkedRenderers = this.GetComponent(typeof(Renderer)) as Renderer;

        coli = GetComponent<Collider>();

        iniRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        if( startAnimate ){
            if( animTimeCnt > animTime ){
                animTimeCnt = animTime;
                startAnimate = false;
            }

            if(_sheet != null){
                for( int i=0 ; i<_linkedRenderers.Length ; i++ ){
                    if( _linkedRenderers[i] == null )
                        continue;
                    _linkedRenderers[i].GetPropertyBlock(_sheet);
                    _sheet.SetFloat("_BlendTest", _animCurve.Evaluate(animTimeCnt / animTime));
                    _linkedRenderers[i].SetPropertyBlock(_sheet);
                }
                
            }

            animTimeCnt += Time.deltaTime;
        }
    }

    void iniRenderer(){
        if( _linkedRenderers == null )
            return;
        
        if(_sheet == null) _sheet = new MaterialPropertyBlock();

        for( int i=0 ; i<_linkedRenderers.Length ; i++ ){
            if( _linkedRenderers[i] == null )
                continue;
            
            _linkedRenderers[i].GetPropertyBlock(_sheet);

            _sheet.SetFloat("_BlendTest", 0);

            _linkedRenderers[i].SetPropertyBlock(_sheet);
        }
        
    }

    public new void initialParam(float timeParam, bool isContinuedMode = false){
        animTime = timeParam;
        continueMode = isContinuedMode;
    }

    public new void doGazeStuff(bool isStart = true){
        // Would Be Setting
        // if( _sheet != null ){
        //     _linkedRenderers.GetPropertyBlock(_sheet);

        //     _sheet.SetFloat("_BrushHeight", this.transform.lossyScale.y * 2.0f);
        //     _sheet.SetVector("_BrushOutShift", new UnityEngine.Vector3(0, -this.transform.lossyScale.y, 0));

        //     _linkedRenderers.SetPropertyBlock(_sheet);
        // }
        startAnimate = true;
        coli.enabled = false;
    }

}
