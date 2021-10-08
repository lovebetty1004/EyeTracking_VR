using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notbeGazed : MonoBehaviour
{
    // Start is called before the first frame update
    public ObjGen objectGen;
    private IEnumerator randomCoroutine;
    void Start()
    {
        // Debug.Log("yayaya");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RandomControl(){
        
        List<GameObject> allObj = objectGen.savedObjs;
        Debug.Log(allObj.Count);
        if(allObj.Count != 0){
            Debug.Log("into if");
            randomCoroutine = randomchoose(allObj);
            if(randomCoroutine != null)
                StopCoroutine(randomCoroutine);
            StartCoroutine(randomCoroutine);
        }
    }
    public IEnumerator randomchoose(List<GameObject> objects){
        int tmp;
        Debug.Log("before while");
        while(true){
            tmp = Random.Range(0, objects.Count);
            Debug.Log(tmp);
            if(objects[tmp].GetComponent<beGazed>().idfLabel != 0)
                continue;
            else{
                IGazeAction gazeAction = objects[tmp].GetComponent(typeof(IGazeAction)) as IGazeAction;
                gazeAction.doGazeStuff(false);
                yield return new WaitForSeconds(2.0f);
            }
        }
    }
}
