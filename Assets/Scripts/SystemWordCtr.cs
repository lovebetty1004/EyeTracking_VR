using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class SystemWordCtr : MonoBehaviour
{
    private TextMesh textWord;
    private float cntDown = 0;
    private Coroutine counter;
    private bool showCntDown = false;
    private string saveWords = "";

    // Start is called before the first frame update
    void Start()
    {
        textWord = this.gameObject.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if( showCntDown ){
            if( cntDown < 0 ){
                cntDown = 0;
                showCntDown = false;
                textWord.text = "";
                return;
            }
            
            textWord.text = saveWords + "\n" + Mathf.CeilToInt(cntDown).ToString();

            cntDown -= Time.deltaTime;
        }
    }

    public void recvWords(string words, float duration = -1, bool showCountDown = false){
        saveWords = words;
        textWord.text = saveWords;
        showCntDown = showCountDown;

        if( duration != -1 ){
            cntDown = duration;

            if( counter != null )
                StopCoroutine(counter);

            counter = StartCoroutine( wordCounter(duration) );
        }
    }

    IEnumerator wordCounter(float duration){
        yield return new WaitForSeconds(duration);
        textWord.text = "";
    }
}
