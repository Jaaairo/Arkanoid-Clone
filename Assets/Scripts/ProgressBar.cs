using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    //variaveis tempo max, tamanho max da barra

    [SerializeField] float maxTime = 30f;
    [SerializeField] Transform bar;
    [SerializeField] TextMeshPro percentageCount;

    float barSizeMax;
    bool isTimerEnabled;

    float timer;
        
    // Start is called before the first frame update
    void Start()
    {
        barSizeMax = bar.localScale.x;

        bar.localScale = new Vector3(0, bar.localScale.y, bar.localScale.z);

        percentageCount.text = "0%";
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerEnabled) return;
     
        timerUpdate();

    }

    void timerUpdate() {
        timer += Time.deltaTime; //equivalente a somar um a cada um segundo
        timer = Mathf.Min(timer, maxTime);

        float percentage = timer / maxTime;

        bar.localScale = new Vector3(percentage*barSizeMax, bar.localScale.y, bar.localScale.z);

        percentageCount.text = (int)(percentage * 100)+"%";
    }

    public void startTimer() {
        isTimerEnabled = true;
        timer = 0;
    }

    public bool isProgressBarFinished() {
        return timer >= maxTime;
    }
}
