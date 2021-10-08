using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class beGazed_brush : beGazed, IGazeAction
{
    // public SHAPE_TYPE shapeType;
    private bool startAnimate = false;
    public float animTime = 1.0f;
    private float animTimeCnt = 0.0f;
    public AnimationCurve _animCurve = AnimationCurve.Linear(0, 0, 1, 1);
    // Start is called before the first frame update

    // [Space]
    // private Collider coli;
	[SerializeField] Renderer _linkedRenderers;
    MaterialPropertyBlock _sheet;

    void Start()
    {
        _linkedRenderers = this.GetComponent(typeof(Renderer)) as Renderer;

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
                coli.enabled = false;   // Close Collider When Finshed Animation
            }

            if(_sheet != null){
                _linkedRenderers.GetPropertyBlock(_sheet);
                _sheet.SetFloat("_BlendTest", _animCurve.Evaluate(animTimeCnt / animTime));
                _linkedRenderers.SetPropertyBlock(_sheet);
            }

            animTimeCnt += Time.deltaTime;
        }
    }

    void iniRenderer(){
        if( _linkedRenderers == null )
            return;
        
        if(_sheet == null) _sheet = new MaterialPropertyBlock();
        _linkedRenderers.GetPropertyBlock(_sheet);

        _sheet.SetFloat("_BlendTest", 0);

        _linkedRenderers.SetPropertyBlock(_sheet);
    }

    public new void initialParam(float timeParam, bool isContinuedMode){
        animTime = timeParam;
        continueMode = isContinuedMode;
    }

    public new void doGazeStuff(bool isStart = true){
        if( startAnimate ){
            if( continueMode ){
                startAnimate = isStart;
                // coli.enabled = !isStart;
            }else{
                return;
            }
        }else{
            startAnimate = isStart;

            if( !continueMode ){
                coli.enabled = !isStart;
            }
        }

        if( _sheet != null ){
            _linkedRenderers.GetPropertyBlock(_sheet);

            _sheet.SetFloat("_BrushHeight", this.transform.lossyScale.y * 2.5f);
            _sheet.SetVector("_BrushOutShift", new UnityEngine.Vector3(0, -this.transform.lossyScale.y, 0));

            _linkedRenderers.SetPropertyBlock(_sheet);
        }
    }

}
