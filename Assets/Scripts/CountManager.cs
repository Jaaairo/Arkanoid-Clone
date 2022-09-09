using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountManager : MonoBehaviour
{
    [SerializeField] AudioManager AM;
    public int startCount;
    int counter;
    [SerializeField] TextMeshPro countTxt;

    public bool isCounterFinished => counter == 0;

    public void startCounter() {
        counter = 3;
        countTxt.text = counter.ToString();
        Invoke(nameof(reduceCounter), 1f);
        AM.Play("counter");
    }

    public void reduceCounter() {
        counter--;
        countTxt.text = counter.ToString();
        AM.Play("counter");
        if(counter > 0) {
            Invoke(nameof(reduceCounter), 1f);
        } else {
            countTxt.text = "";
        }
    }

}
