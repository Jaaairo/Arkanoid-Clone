using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
public class BallController : MonoBehaviour
{

    public TrailRenderer ballTrail;

    public event Action<BallController> onDeath; //Action é um delegate (evento)
    
    //limite da tela
    private Vector2 screenBounds;
    private float ballW;
    private float ballH;
    Vector3 viewPos;

    public float FinalSpeed=>speed * ballSpeedMulti; //Atributo, quando chamado faz o cálculo e retorna o valor
    public float speed = 5f;
    public Vector2 direction;
    float radius;
    float startSpd ;
    float spdLimit = 15f;
    Vector3 startPosition;

    public float camShakeVelocity => speed * 0.02f;

    public GameManager GM;

    public float ballSpeedMulti=1;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startSpd = speed;

        radius = transform.localScale.x / 2;

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        ballW = transform.GetComponent<SpriteRenderer>().bounds.size.x;
        ballH = transform.GetComponent<SpriteRenderer>().bounds.size.y;
        
        gameStart();
       
    }

    // Update is called once per frame
    void Update()
    {
        viewPos = transform.position;       
        viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + ballW / 2, screenBounds.x - ballW / 2);
        viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + ballH / 2, screenBounds.y - ballH / 2);       
        transform.position = viewPos;

        transform.Translate(direction * FinalSpeed * Time.deltaTime);

        ballBounce();

    }

    private void ballBounce() {
        var topLeft = (Vector2) Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight));
        var bottomRight = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0));

        if (transform.position.y < -topLeft.y + radius && direction.y < 0) {
            GM.camShake(camShakeVelocity);
            onDeath.Invoke(this);
            GM.playSound("ballExplode");
        }
        else if (transform.position.y > topLeft.y - radius && direction.y > 0) {
            GM.camShake(camShakeVelocity);
            direction.y *= -1;
            GM.playSound("wallHit");
        }
        if (transform.position.x < -bottomRight.x + radius && direction.x < 0) {
            GM.camShake(camShakeVelocity);
            direction.x *= -1;
            GM.playSound("wallHit");
        } else if (transform.position.x > bottomRight.x - radius && direction.x > 0) {
            GM.camShake(camShakeVelocity);
            direction.x *= -1;
            GM.playSound("wallHit");
        }
    }

    public void gameStart() {
        
        ballTrail.GetComponent<Renderer>().enabled = true;

        resetSpeed();

        int randInt = Random.Range(0, 2);
        if (randInt == 0) {
            randInt = -1;
        }
        
        direction = new Vector2(randInt, -1);
    }

    public void resetPosition() {
        ballTrail.time = 0.2f;
        ballTrail.GetComponent<Renderer>().enabled = false;
        speed = 0;
        transform.position = startPosition;
    }

    public void resetSpeed() {
        speed = startSpd;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out PadController pad)) {
            direction.y = -direction.y;
            speed += 1f;
            direction.x = transform.position.x - pad.transform.position.x;
            //direction.Normalize();
            direction.x += pad.padVelocity;
            direction.x = Mathf.Clamp(direction.x, -1.5f, 1.5f);

            //direction.Normalize(); // Normaliza o vetor para deixar no tamanho maximo de 1

            transform.localScale = transform.localScale + GM.scaleChange;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Min(scale.x, 0.4f);
            scale.y = Mathf.Min(scale.y, 0.4f);
            scale.z = Mathf.Min(scale.z, 0.4f);

            transform.localScale = scale;

            ballTrail.time += 0.10f;
            speed = Mathf.Clamp(speed, 0, spdLimit);


            GM.addHit();
            GM.playSound("padHit");
            GM.camShake(camShakeVelocity);
        }else if (other.TryGetComponent(out Block block)) {
            block.damage();
            GM.playSound("padHit");
            GM.camShake(camShakeVelocity);
            direction.y = -direction.y;
        }
        
        

    }
    




}
