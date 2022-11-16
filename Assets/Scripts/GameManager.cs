using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [SerializeField] PadController padCTR; //Cria uma vari�vel que acessa a classe PadController.
    [SerializeField] AudioManager AM; //Cria uma vari�vel que acessa a classe PadController.
    [SerializeField] BallController ballPrefab; //Cria uma vari�vel que armazena o prefab da bola para podermos ter mais op��es de uso do game object
    [SerializeField] TextMeshPro shiftTxt; //Vari�vel usada para acessar o texto do shift time.
    [SerializeField] TextMeshPro shiftTxtBG;
    [SerializeField] CountManager CountManager;
    [SerializeField] CameraManager camManager;
    [SerializeField] BlockManager blockManager;
    [SerializeField] ProgressBar progressBarCTR;

    [SerializeField] AnimationCurve shiftLevel = AnimationCurve.Linear(0,0,1,1);

    List<BallController> ballControllers = new List<BallController>(); //Lista utilizada para contar quantas bolinhas tem na tela

    PadActions mapControls; //Vari�vel com o mapa de inputs

    public int shiftCount; //Vari�vel usada para armazenar o n�mero de colis�es entre as bolinhas e o pad

    public Vector3 scaleChange = new Vector3(0.005f, 0.005f, 0.005f);

    bool gameIsRunning;
    bool gameInitialized;
    bool isShift;
    float shiftTimer;
    public float updateShakeAmount;
    float padSpeed;

    int hitCount;
    int ballInstaceCount;

    private void Awake() {
        padSpeed = padCTR.speed;
        mapControls = new PadActions(); //Armazena o mapa de a��es
        setShiftCount(0); //Inicia o jogo com a contagem de colis�es em zero
        restartScene();
        blockManager.spawnBlocks();
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
        if (ballControllers.Count==0 && gameIsRunning==true){
            padSpeed = 0;
            restartScene();
        }

        if (CountManager.isCounterFinished && gameIsRunning == false) { //Enquanto o valor de couter for menor que tr�s
            gameReset(); //Chama o m�todo gameStart
        }

        shiftCountReduce();

        if(isShift == true) {
            gravityPull();
        }

        if (gameInitialized && progressBarCTR.isProgressBarFinished()) {
            if (gameIsRunning) {
                destroyAllBalls();
            }
            gameOver();
        }
    }


    //M�todo que inicia o jogo 
    void gameReset() {
        padCTR.resetSpeed();
        ballInstance();
        AM.Play("gameStart");
        gameIsRunning = true;

        if (!gameInitialized) {
            gameStart();
        }
    }

    void gameStart() {
        progressBarCTR.startTimer();
        blockManager.spawnBlocks();
        gameInitialized = true;
    }

    void gameOver() {
        gameInitialized = false;
        progressBarCTR.resetProgressBar();
    }

    //M�todo para criar bolas na tela
    public void ballInstance() {
        var ballInstance = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity); //Armazena uma inst�ncia do prefab na vari�vel. N�o sei o que significa identity.
        ballControllers.Add(ballInstance); //adiciona uma inst�ncia na lista

        ballInstance.GM = this; //Adiciona uma c�pia do game manager a essa (this) inst�ncia. A vari�vel GM est� sendo usada na checagem de colis�o na classe BallController
        
        ballInstance.GM.updateShakeAmount = ballInstance.speed;

        ballInstance.onDeath += destroyBall; //onDeath est� sendo chamado da classe BallController mas n�o entendi direito o que �
    }

    //M�todo para lidar com a destrui��o das bolas
    void destroyBall(BallController ball) {
        ballControllers.Remove(ball);
        Destroy(ball.gameObject);
        if (ballControllers.Count == 0) {
            ballsEnded();
        }
    }

    //M�todo que reseta o jogo
    private void ballsEnded() {
        setShiftCount(0);
        hitCount = 0;
        ballInstaceCount = 0;
        AM.Play("gameOver");
    }

    //M�todo que termina o jogo se a barra de progresso chega ao fim 
    void destroyAllBalls() {
        for (int i = ballControllers.Count - 1; i >= 0; i--) {
            destroyBall(ballControllers[i]);                    
        }
    }

    //M�todo que ativa o Shift Time
    public void setShiftCount(int hitCount) {

        int startShiftCount = shiftCount;

        shiftCount = Mathf.Max(hitCount, 0); // Max escolhe o valor mais alto entre hitCount e zero.
        shiftTxt.text = shiftCount.ToString();
        shiftTxtBG.text = "";

        shiftTxt.gameObject.SetActive(shiftCount > 0);

        for (int i = 0; i < shiftTxt.text.Length; i++) { //O for tem tr�s partes = Inicializa��o / condi��o / Incrementa��o - Lenght � o comprimento (numero de caracteres) da string
            shiftTxtBG.text += "0"; // i = 0 conteudo = "" texto = "0" // i = 1 conteudo = "0" texto = "00"
        }
    }

    //M�todo que aumenta a velocidade das bolas
    void addSpeed(InputAction.CallbackContext ctx) {
        if (shiftCount > 0) {
            foreach (BallController ball in ballControllers) {       
                ball.ballSpeedMulti = 0.5f;
                padCTR.shiftSpeed = 0.8f;
            }
            isShift = true;           
        }
    }

    void resetSpeed(InputAction.CallbackContext ctx) {
        resetSpeed();
    }
    void resetSpeed() {
        foreach (BallController ball in ballControllers) {
            ball.ballSpeedMulti = 1;
        }
        padCTR.shiftSpeed = 1;
        isShift = false;
    }

    void restartScene() {
        padCTR.resetPosition(); //Chama o m�todo resetPosition na classe padController
        padCTR.padSpeed(0.5f); //chama o m�todo padSpeed na classe padController e passa o valor determinado
        CountManager.startCounter(); 
        gameIsRunning = false;
    }

    public void playSound(string name) {
        AM.Play(name);
    }

    public void camShake(float addShakeAmount) {
        camManager.ShakeCamera(addShakeAmount);
    }

    void shiftCountReduce() {
        if(isShift == true) {
            camManager.ShakeCamera(0.02f);
            shiftTimer += Time.deltaTime*1.5f;
            if(shiftTimer >= 1) {
                setShiftCount(shiftCount - 1);
                shiftTimer = 0;
                if(shiftCount == 0) {
                    resetSpeed();
                }
            }
        } else {
            shiftTimer = 0;
        }
    }

    void gravityPull() {
        foreach (BallController ball in ballControllers) {
            if(ball.direction.y < 0) {
                Vector3 targetDir = ball.transform.position - padCTR.transform.position;
                ball.direction = ball.direction - (Vector2) targetDir * 1f * Time.deltaTime;
                ball.direction.Normalize();
            }
        }
    }

    public void addHit() {
        setShiftCount(shiftCount + 1);
        hitCount++;

        if (hitCount % Mathf.RoundToInt(shiftLevel.Evaluate(ballInstaceCount)) == 0) {
            ballInstance();
            ballInstaceCount++;
        }
    }

}
