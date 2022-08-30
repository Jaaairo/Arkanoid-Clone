using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [SerializeField] PadController padCTR; //Cria uma vari�vel que acessa a classe PadController.
    [SerializeField] BallController ballPrefab; //Cria uma vari�vel que armazena o prefab da bola para podermos ter mais op��es de uso do game object
    [SerializeField] TextMeshPro countTxt; //Vari�vel usada para acessar o texto do counter
    [SerializeField] TextMeshPro shiftTxt; //Vari�vel usada para acessar o texto do shift time.

    float counter; //Vari�vel com o valor do counter
    bool resetPadPos; //Vari�vel booleana determina se a posi��o do pad vai ser resetada

    List<BallController> ballControllers = new List<BallController>(); //Lista utilizada para contar quantas bolinhas tem na tela

    PadActions mapControls; //Vari�vel com o mapa de inputs

    public int shiftCount; //Vari�vel usada para armazenar o n�mero de colis�es entre as bolinhas e o pad

    public Vector3 scaleChange = new Vector3(0.005f, 0.005f, 0.005f); 

    private void Awake() {
        mapControls = new PadActions(); //Armazena o mapa de a��es
        setShiftCount(0); //Inicia o jogo com a contagem de colis�es em zero
    }

    private void OnEnable() {
        mapControls.Enable();
        mapControls.PadActionMap.buttonMods.performed += addSpeed; //ativa o m�todo addSpeed quando o comando de shift � acionado //Delegate? Uma vari�vel que vai guardar uma lista de m�todos
        mapControls.PadActionMap.buttonMods.canceled += resetSpeed; //ativa o m�todo resetSpeed quando o comando de shift � acionado
    }

    private void OnDisable() {
        mapControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Se o n�mero de ballControllers (bolas instanciadas) chegar a zero a fun��o updateCounter � chamada
        if (ballControllers.Count==0){
            updateCounter();
            if (!resetPadPos) { //Se resetPadPos for falsa...
                padCTR.resetPosition(); //Chama o m�todo resetPosition na classe padController
                resetPadPos = true; //Muda o estado da vari�vel para verdadeiro
                padCTR.padSpeed(0.5f); //chama o m�todo padSpeed na classe padController e passa o valor determinado
            }
        }

        if (counter > 3) { //Enquanto o valor de canter for menor que tr�s
            counter = 0; //Zera a var�vel
            gameStart(); //Chama o m�todo gameStart
        }
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

    //M�todo para criar bolas na tela
    public void ballInstance() {
        var ballInstance = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity); //Armazena uma inst�ncia do prefab na vari�vel. N�o sei o que significa identity.
        ballControllers.Add(ballInstance); //adiciona uma inst�ncia na lista

        ballInstance.GM = this; //Adiciona uma c�pia do game manager a essa (this) inst�ncia. A vari�vel GM est� sendo usada na checagem de colis�o na classe BallController

        ballInstance.onDeath += destroyBall; //onDeath est� sendo chamado da classe BallController mas n�o entendi direito o que �
    }

    //M�todo para lidar com a destrui��o das bolas
    void destroyBall(BallController ball) {
        ballControllers.Remove(ball);
        Destroy(ball.gameObject);
        if (ballControllers.Count == 0) {
            setShiftCount(0);
        }
    }

    public void setShiftCount(int hitCount) {

        shiftCount = Mathf.Max(hitCount, 0); // Max escolhe o valor mais alto entre hitCount e zero.
        shiftTxt.text = shiftCount.ToString();

        shiftTxt.gameObject.SetActive(shiftCount > 0);

        if (shiftCount == 5) {
            ballInstance();
        }
    }


    void addSpeed(InputAction.CallbackContext ctx) {
        if (shiftCount > 0) {
            foreach (BallController ball in ballControllers) {       
                ball.ballSpeedMulti = 0.5f;
                padCTR.speed *= 0.8f;
            }
            setShiftCount(shiftCount-1); //Reduzir por segundos se bot�o permanecer pressionado
        }
    }

    void resetSpeed(InputAction.CallbackContext ctx) {
        foreach (BallController ball in ballControllers) {
            ball.ballSpeedMulti = 1;
        }
        padCTR.resetSpeed();
    }


}
