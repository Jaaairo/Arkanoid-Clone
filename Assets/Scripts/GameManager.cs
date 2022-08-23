using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [SerializeField] PadController padCTR;
    [SerializeField] BallController ballPrefab;
    [SerializeField] TextMeshPro countTxt;
    [SerializeField] TextMeshPro shiftTxt;

    float counter;
    bool resetPadPos;

    List<BallController> ballControllers = new List<BallController>();

    PadActions mapControls;

    public int shiftCount;

    private void Awake() {
        mapControls = new PadActions();
        setShiftCount(0);
    }

    private void OnEnable() {
        mapControls.Enable();
        mapControls.PadActionMap.buttonMods.performed += addSpeed;
        mapControls.PadActionMap.buttonMods.canceled += resetSpeed;
    }

    private void OnDisable() {
        mapControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballControllers.Count==0){
            updateCounter();

            if (!resetPadPos) {
                padCTR.resetPosition();
                resetPadPos = true;
                padCTR.padSpeed(0.5f);
            }


        }

        if (counter > 3) {
            //texto na tela
            counter = 0;
            gameStart();
        }

    }

    void ballInstance() {
        var ballInstance = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        ballControllers.Add(ballInstance);
        
        ballInstance.GM = this;

        ballInstance.onDeath += destroyBall;
    }

    void destroyBall(BallController ball) {
        ballControllers.Remove(ball);
        Destroy(ball.gameObject);

        setShiftCount(0);
    }

    private void updateCounter() {
        counter = counter + Time.deltaTime;
        int txtInt = 3 - (int)counter;
        countTxt.text = txtInt.ToString();
    }

    void gameStart() {
        countTxt.text = "";
        padCTR.resetSpeed();
        resetPadPos = false;
        ballInstance();
    }
    void addSpeed(InputAction.CallbackContext ctx) {
        foreach (BallController ball in ballControllers) {       
            ball.ballSpeedMulti = 0.5f;
        }
        padCTR.speed = padCTR.speed * 0.8f;
        setShiftCount(shiftCount-1);

    }
    void resetSpeed(InputAction.CallbackContext ctx) {
        foreach (BallController ball in ballControllers) {
            ball.ballSpeedMulti = 1;
        }
        padCTR.resetSpeed();
    }

    public void setShiftCount(int hitCount) {

        shiftCount = Mathf.Max(hitCount, 0);
        shiftTxt.text = shiftCount.ToString();

        shiftTxt.gameObject.SetActive(shiftCount>0);
    }
}
