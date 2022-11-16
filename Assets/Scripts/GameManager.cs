using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [SerializeField] PadController padCTR; //Cria uma variável que acessa a classe PadController.
    [SerializeField] AudioManager AM; //Cria uma variável que acessa a classe PadController.
    [SerializeField] BallController ballPrefab; //Cria uma variável que armazena o prefab da bola para podermos ter mais opções de uso do game object
    [SerializeField] TextMeshPro shiftTxt; //Variável usada para acessar o texto do shift time.
    [SerializeField] TextMeshPro shiftTxtBG;
    [SerializeField] CountManager CountManager;
    [SerializeField] CameraManager camManager;
    [SerializeField] BlockManager blockManager;
    [SerializeField] ProgressBar progressBarCTR;

    [SerializeField] AnimationCurve shiftLevel = AnimationCurve.Linear(0,0,1,1);

    List<BallController> ballControllers = new List<BallController>(); //Lista utilizada para contar quantas bolinhas tem na tela

    PadActions mapControls; //Variável com o mapa de inputs

    public int shiftCount; //Variável usada para armazenar o número de colisões entre as bolinhas e o pad

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
        mapControls = new PadActions(); //Armazena o mapa de ações
        setShiftCount(0); //Inicia o jogo com a contagem de colisões em zero
        restartScene();
        blockManager.spawnBlocks();
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
        if (ballControllers.Count==0 && gameIsRunning==true){
            padSpeed = 0;
            restartScene();
        }

        if (CountManager.isCounterFinished && gameIsRunning == false) { //Enquanto o valor de couter for menor que três
            gameReset(); //Chama o método gameStart
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


    //Método que inicia o jogo 
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

    //Método para criar bolas na tela
    public void ballInstance() {
        var ballInstance = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity); //Armazena uma instância do prefab na variável. Não sei o que significa identity.
        ballControllers.Add(ballInstance); //adiciona uma instância na lista

        ballInstance.GM = this; //Adiciona uma cópia do game manager a essa (this) instância. A variável GM está sendo usada na checagem de colisão na classe BallController
        
        ballInstance.GM.updateShakeAmount = ballInstance.speed;

        ballInstance.onDeath += destroyBall; //onDeath está sendo chamado da classe BallController mas não entendi direito o que é
    }

    //Método para lidar com a destruição das bolas
    void destroyBall(BallController ball) {
        ballControllers.Remove(ball);
        Destroy(ball.gameObject);
        if (ballControllers.Count == 0) {
            ballsEnded();
        }
    }

    //Método que reseta o jogo
    private void ballsEnded() {
        setShiftCount(0);
        hitCount = 0;
        ballInstaceCount = 0;
        AM.Play("gameOver");
    }

    //Método que termina o jogo se a barra de progresso chega ao fim 
    void destroyAllBalls() {
        for (int i = ballControllers.Count - 1; i >= 0; i--) {
            destroyBall(ballControllers[i]);                    
        }
    }

    //Mètodo que ativa o Shift Time
    public void setShiftCount(int hitCount) {

        int startShiftCount = shiftCount;

        shiftCount = Mathf.Max(hitCount, 0); // Max escolhe o valor mais alto entre hitCount e zero.
        shiftTxt.text = shiftCount.ToString();
        shiftTxtBG.text = "";

        shiftTxt.gameObject.SetActive(shiftCount > 0);

        for (int i = 0; i < shiftTxt.text.Length; i++) { //O for tem três partes = Inicialização / condição / Incrementação - Lenght é o comprimento (numero de caracteres) da string
            shiftTxtBG.text += "0"; // i = 0 conteudo = "" texto = "0" // i = 1 conteudo = "0" texto = "00"
        }
    }

    //Método que aumenta a velocidade das bolas
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
        padCTR.resetPosition(); //Chama o método resetPosition na classe padController
        padCTR.padSpeed(0.5f); //chama o método padSpeed na classe padController e passa o valor determinado
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
