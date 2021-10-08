using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class beGazed1 : MonoBehaviour, IGazeAction
{  
    public enum _FW_FACE{
        _X,
        _Z
    }
    public int idfLabel = 0;
    public SHAPE_TYPE shapeType;
    public _FW_FACE fwFace = _FW_FACE._Z;
    private MeshRenderer mRender;
    protected Collider coli;
    protected float areaPercent = 1;

    public bool continueMode = false;

    // public bool TriggerByEye = false;
    public Material highlightMaterial;
    public enum Mode {Light, Shake, Sound};
    public Mode EXPMode;
    private Material defaultMaterial;

    private float delaytime;
    private bool lighting = false;
    private bool shake = false;
    private bool sound = false;

    private IEnumerator lightcoroutine;
    private IEnumerator shakecoroutine;
    private IEnumerator soundcoroutine;

    private IEnumerator randomlight;
    private IEnumerator randomshake;
    private IEnumerator randomsound;

    private Vector3 _originalPos;
    private bool shakeflag = true;
    
    // Start is called before the first frame update
    void Start()
    {
        mRender = GetComponent<MeshRenderer>();
        mRender.enabled = true;
        defaultMaterial = mRender.sharedMaterial;
        coli = GetComponent<Collider>();
        lighting = false;
        shake = false;
        sound = false;
        switch(EXPMode){
			case Mode.Light:
				lighting = true;
				break;
			case Mode.Shake:
				shake = true;
				break;
			case Mode.Sound:
				sound= true;
				break;
		}
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    public void initialParam(float timeParam, bool isContinuedMode = false){
            this.delaytime = timeParam;
            Debug.Log("DelayTime"+ delaytime);
    }
    public int identifyLabel(){
        return idfLabel;
    }
    public void doGazeStuff(bool isStart = true){
        // mRender.enabled = true;
        // coli.enabled = false;
        if(isStart && this.identifyLabel() != 0 && lighting){
            coli.enabled = false;
            lightcoroutine = changeMaterial();
            if(lightcoroutine != null)
                StopCoroutine(lightcoroutine);
            StartCoroutine(lightcoroutine);
        }
        if(isStart&& this.identifyLabel() != 0 && shake && shakeflag){
            coli.enabled = false;
            shakeflag = false;
            Shake(1, 0.5f, delaytime);
        }
        if(isStart && this.identifyLabel() != 0 && sound){
            coli.enabled = false;
            soundcoroutine = changeSound(delaytime);
            if(soundcoroutine != null)
                StopCoroutine(soundcoroutine);
            StartCoroutine(soundcoroutine);
        }
        if(!isStart && this.identifyLabel() == 0 && lighting){
            randomlight = changeforonesecondlight();
            if(randomlight != null)
                StopCoroutine(randomlight);
            StartCoroutine(randomlight);
        }
        if(!isStart && this.identifyLabel() == 0 && shake){
            shakeflag = false;
            Shake(1, 0.5f, 0);
        }
        if(!isStart && this.identifyLabel() == 0 && sound){
            randomsound = changeSound(0);
            if(randomsound != null)
                StopCoroutine(randomsound);
            StartCoroutine(randomsound);
        }
        
    }

    public void adjustCollider(float nearPlaneArea, Vector3 refPos, float basedPercent = 0.0f){
        // NearPlaneDistance will be divided by nearPlaneArea, so set to 1.0f
        float distScale = 1.0f / Vector3.Distance(refPos, this.transform.position + this.transform.forward * fwScale());
        float nearColliArea = findCollidArea(distScale);
        areaPercent = nearColliArea / nearPlaneArea;
        
        if( basedPercent > 0 && basedPercent > areaPercent ){
            // Debug.Log(this.gameObject.name + ": <b>" + (areaPercent * 100).ToString("F2") + "%</b>", this.gameObject);
            
            enlargeCollider( Mathf.Sqrt( basedPercent / areaPercent ));
        }
    }

    public void infoCopyer(ref Dictionary<int, Vector3> booker){
        if( booker.ContainsKey(this.gameObject.GetInstanceID()) ){
            Vector3 tmp = booker[this.gameObject.GetInstanceID()];
            
            // Means Already Triggered?
            // Temp Code //
            tmp.y = 1.0f;

            booker[this.gameObject.GetInstanceID()] = tmp;
        }else{
            booker.Add(this.gameObject.GetInstanceID(), new Vector3(areaPercent, 0, 0));
        }
    }

    private float fwScale(){
        switch(shapeType){
            case SHAPE_TYPE._Box:
                BoxCollider getBox = this.gameObject.GetComponent<BoxCollider>();

                return ( fwFace == _FW_FACE._X ) ? 
                    getBox.size.x * 0.5f * this.transform.lossyScale.x * 0.5f: 
                    getBox.size.z * 0.5f * this.transform.lossyScale.z * 0.5f;
            case SHAPE_TYPE._Sphere:
                SphereCollider getSphere = this.gameObject.GetComponent<SphereCollider>();

                return getSphere.radius * this.transform.lossyScale.x * 0.5f;
            case SHAPE_TYPE._Capsule:
                CapsuleCollider getCap = this.gameObject.GetComponent<CapsuleCollider>();
                return getCap.radius * this.transform.lossyScale.x * 0.5f;
            default:
                return 1;
        }
    }

    public float findCollidArea(float distScale = 1.0f){
        switch(shapeType){
            case SHAPE_TYPE._Box:
                BoxCollider getBox = this.gameObject.GetComponent<BoxCollider>();
                float colliderArea = ( fwFace == _FW_FACE._X ) ? getBox.size.y * getBox.size.z : getBox.size.y * getBox.size.x;
                float objectArea = ( fwFace == _FW_FACE._X ) ? this.transform.lossyScale.y * this.transform.lossyScale.z : this.transform.lossyScale.y * this.transform.lossyScale.x;

                return  objectArea * colliderArea * distScale * distScale;
            case SHAPE_TYPE._Sphere:
                // SphereCollider getSphere = this.gameObject.GetComponent<SphereCollider>();

                return this.transform.lossyScale.x * this.transform.lossyScale.x * 0.25f * Mathf.PI * distScale * distScale;
            case SHAPE_TYPE._Capsule:
                return this.transform.lossyScale.x * this.transform.lossyScale.y * distScale * distScale + 
                        this.transform.lossyScale.x * this.transform.lossyScale.x * 0.25f * Mathf.PI * distScale * distScale;
        }
        return 1.0f;
    }

    public void enlargeCollider(float enlargeSize){
        try{
            switch(shapeType){  // Shifting Depth Issue to ensure THE SAME Area Percentage related to original one
                case SHAPE_TYPE._Box:
                    BoxCollider getBox = this.gameObject.GetComponent<BoxCollider>();
                    // getBox.size = new Vector3(enlargeSize, enlargeSize, 1);

                    getBox.size = ( fwFace == _FW_FACE._X ) ? 
                        Vector3.Scale(getBox.size, new Vector3(1, enlargeSize, enlargeSize)) :
                        Vector3.Scale(getBox.size, new Vector3(enlargeSize, enlargeSize, 1));
                    break;
                case SHAPE_TYPE._Sphere:
                    SphereCollider getSphere = this.gameObject.GetComponent<SphereCollider>();
                    getSphere.radius = 0.5f * enlargeSize;
                    getSphere.center = new Vector3(0, 0, -enlargeSize * 0.5f);
                    break;
                case SHAPE_TYPE._Capsule:
                    CapsuleCollider getCap = this.gameObject.GetComponent<CapsuleCollider>();
                    getCap.height = 2 * enlargeSize;
                    getCap.radius = 0.5f * enlargeSize;
                    getCap.center = new Vector3(0, 0, -enlargeSize * 0.5f);
                    break;
            }
        } catch(Exception e){
            Debug.Log(e.Message);
        }
        
    }

    public void resetCollider(){
        try{
            switch(shapeType){
                case SHAPE_TYPE._Box:
                    BoxCollider getBox = this.gameObject.GetComponent<BoxCollider>();
                    getBox.size = Vector3.one;
                    break;
                case SHAPE_TYPE._Sphere:
                    SphereCollider getSphere = this.gameObject.GetComponent<SphereCollider>();
                    getSphere.radius = 0.5f;
                    getSphere.center = Vector3.zero;
                    break;
                case SHAPE_TYPE._Capsule:
                    CapsuleCollider getCap = this.gameObject.GetComponent<CapsuleCollider>();
                    getCap.height = 2;
                    getCap.radius = 0.5f;
                    getCap.center = Vector3.zero;
                    break;
            }
        } catch(Exception e){
            Debug.Log(e.Message);
        }
    }
    //Add By Betty for doGaze
    private IEnumerator changeforonesecondlight ()
    {
        this.GetComponent<Renderer>().sharedMaterial = highlightMaterial;
        yield return new WaitForSeconds(1f);
        this.GetComponent<Renderer>().sharedMaterial = defaultMaterial;
    }
    private IEnumerator changeMaterial()
    {
        Debug.Log("before delay");
        yield return new WaitForSeconds(delaytime);
        Debug.Log("After Delay");
        this.GetComponent<Renderer>().sharedMaterial = highlightMaterial;
        yield return new WaitForSeconds(1f);
        this.GetComponent<Renderer>().sharedMaterial = defaultMaterial;
    }
    private IEnumerator changeSound (float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip, 0.5f);
    }
    public void Shake (float duration, float amount, float delayTime) {
        _originalPos = transform.localPosition;
        shakecoroutine = cShake(duration, amount, delayTime);
        if(shakecoroutine != null)
            StopCoroutine(shakecoroutine);
        StartCoroutine(shakecoroutine);
    }
    public IEnumerator cShake (float duration, float amount, float delayTime) {
        if(!shakeflag)
        {
            yield return new WaitForSeconds(delayTime);
            // Debug.Log("start moveing" + delayTime);
            float endTime = Time.time + duration;
            float oriDuration = duration;

            while (Time.time < endTime) {
                transform.localPosition = _originalPos + UnityEngine.Random.insideUnitSphere * amount * duration / oriDuration;

                duration -= Time.deltaTime;

                yield return null;
            }

            transform.localPosition = _originalPos;
            shakeflag = true;
        }
    }
}
