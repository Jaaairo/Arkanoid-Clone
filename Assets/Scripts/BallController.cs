using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
public class BallController : MonoBehaviour
{
    public GameObject ball;
    public TrailRenderer ballTrail;

    public Action<BallController> onDeath;
    
    //limite da tela
    private Vector2 screenBounds;
    private float ballW;
    private float ballH;
    Vector3 viewPos;

    public float FinalSpeed=>speed * ballSpeedMulti; //Atributo
    public float speed = 5;
    Vector2 direction;
    float radius;
    float startSpd ;
    float spdLimit = 15f;
    Vector3 startPosition;

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

        var topLeft = (Vector2) Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight,Camera.main.nearClipPlane));
        var bottomRight = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, Camera.main.nearClipPlane));

        if (transform.position.y < -topLeft.y + radius && direction.y < 0) {
            onDeath.Invoke(this);
        }
        else if (transform.position.y > topLeft.y - radius && direction.y > 0) {
            direction.y = -direction.y;
        }

        if (transform.position.x < -bottomRight.x + radius && direction.x < 0) {
            direction.x = -direction.x;
        } else if (transform.position.x > bottomRight.x - radius && direction.x > 0) {
            direction.x = -direction.x;
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
        if (other.CompareTag("Pad")) {
            direction.y = -direction.y;
            speed = speed + 1;
            ballTrail.time += 0.10f;
            speed = Mathf.Clamp(speed, 0, spdLimit);

            GM.setShiftCount(GM.shiftCount+1);
        }
    }
    




}
