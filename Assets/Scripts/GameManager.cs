using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [SerializeField] PadController padCTR; //Cria uma variável que acessa a classe PadController.
    [SerializeField] BallController ballPrefab; //Cria uma variável que armazena o prefab da bola para podermos ter mais opções de uso do game object
    [SerializeField] TextMeshPro countTxt; //Variável usada para acessar o texto do counter
    [SerializeField] TextMeshPro shiftTxt; //Variável usada para acessar o texto do shift time.

    float counter; //Variável com o valor do counter
    bool resetPadPos; //Variável booleana determina se a posição do pad vai ser resetada

    List<BallController> ballControllers = new List<BallController>(); //Lista utilizada para contar quantas bolinhas tem na tela

    PadActions mapControls; //Variável com o mapa de inputs

    public int shiftCount; //Variável usada para armazenar o número de colisões entre as bolinhas e o pad

    public Vector3 scaleChange = new Vector3(0.005f, 0.005f, 0.005f); 

    private void Awake() {
        mapControls = new PadActions(); //Armazena o mapa de ações
        setShiftCount(0); //Inicia o jogo com a contagem de colisões em zero
    }

    private void OnEnable() {
        mapControls.Enable();
        mapControls.PadActionMap.buttonMods.performed += addSpeed; //ativa o método addSpeed quando o comando de shift é acionado //Delegate? Uma variável que vai guardar uma lista de métodos
        mapControls.PadActionMap.buttonMods.canceled += resetSpeed; //ativa o método resetSpeed quando o comando de shift é acionado
    }

    private void OnDisable() {
        mapControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Se o número de ballControllers (bolas instanciadas) chegar a zero a função updateCounter é chamada
        if (ballControllers.Count==0){
            updateCounter();
            if (!resetPadPos) { //Se resetPadPos for falsa...
                padCTR.resetPosition(); //Chama o método resetPosition na classe padController
                resetPadPos = true; //Muda o estado da variável para verdadeiro
                padCTR.padSpeed(0.5f); //chama o método padSpeed na classe padController e passa o valor determinado
            }
        }

        if (counter > 3) { //Enquanto o valor de canter for menor que três
            counter = 0; //Zera a varável
            gameStart(); //Chama o método gameStart
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

    //Método para criar bolas na tela
    public void ballInstance() {
        var ballInstance = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity); //Armazena uma instância do prefab na variável. Não sei o que significa identity.
        ballControllers.Add(ballInstance); //adiciona uma instância na lista

        ballInstance.GM = this; //Adiciona uma cópia do game manager a essa (this) instância. A variável GM está sendo usada na checagem de colisão na classe BallController

        ballInstance.onDeath += destroyBall; //onDeath está sendo chamado da classe BallController mas não entendi direito o que é
    }

    //Método para lidar com a destruição das bolas
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
            setShiftCount(shiftCount-1); //Reduzir por segundos se botão permanecer pressionado
        }
    }

    void resetSpeed(InputAction.CallbackContext ctx) {
        foreach (BallController ball in ballControllers) {
            ball.ballSpeedMulti = 1;
        }
        padCTR.resetSpeed();
    }


}
