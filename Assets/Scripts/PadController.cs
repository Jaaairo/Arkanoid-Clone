using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PadController : MonoBehaviour
{
    private float speed;
    public float airDrag = 0.5f;
    public float spdLimit = 75f;
    private float acceleration;
    private float velocity;
    public float startSpeed;
    public float shiftSpeed=1;
    public float finalPadSpeed => speed * shiftSpeed;
    public float padVelocity => velocity;

    Rigidbody2D pad;
    //public GameObject padd;

    //limite da tela
    private Vector2 screenBounds;
    private float padW;

    //Input System
    PadActions mapControls;

    Vector3 startPosition;

    private void Awake() {
        mapControls = new PadActions();
    }

    void OnEnable() {
        mapControls.PadActionMap.Enable();
    }

    void OnDisable() {
        mapControls.PadActionMap.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        pad = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        padW = transform.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        padMove();
    }

    private void padMove() {
        var axisX = mapControls.PadActionMap.Move.ReadValue<Vector2>();

        if (axisX.x <= -1) {

            if (velocity > 0) {
                velocity = 0;
                acceleration = 0;
            }
            //transform.Translate(Vector2.right * -speed * Time.deltaTime);
            acceleration = -finalPadSpeed;
        }

        else if (axisX.x >= 1) {
            if (velocity < 0) {
                velocity = 0;
                acceleration = 0;
            }
            //transform.Translate(Vector2.right * speed * Time.deltaTime);
            acceleration = finalPadSpeed;
        } 
        
        else {
            acceleration = 0;
        }

        velocity += acceleration * Time.deltaTime;
        if (acceleration == 0) {
            velocity *= 1 - airDrag * Time.deltaTime;
        }

        velocity = Mathf.Clamp(velocity, -spdLimit, spdLimit);

        transform.Translate(new Vector2(velocity, 0));

        // Checando o limite da tela
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + padW / 2, screenBounds.x - padW / 2);
        transform.position = viewPos;
    }

    public void padSpeed(float spd) {
        speed = spd;
    }

    public void resetSpeed() {
        speed = startSpeed;
        shiftSpeed = 1;
    }

    public void resetPosition() {
        if (startPosition == Vector3.zero) {
            startPosition = transform.position;
        } else {
            transform.position = startPosition;
        }
    }

}