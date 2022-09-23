using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Camera Information
    public Transform cameraTransform;
    private Vector3 orignalCameraPos;

    // Shake Parameters
    public float shakeDuration;
    public float shakeAmount;

    private bool canShake = false;
    private float _shakeTimer;

    public float updateShakeAmount;



    // Start is called before the first frame update
    void Start() {
        orignalCameraPos = cameraTransform.localPosition;
    }

    // Update is called once per frame  
    void Update() {

        if (canShake) {
            StartCameraShakeEffect();
        }
    }

    public void ShakeCamera(float addShakeAmount) {
        canShake = true;
        _shakeTimer = shakeDuration;
        updateShakeAmount = addShakeAmount + shakeAmount;
    }

    public void StartCameraShakeEffect() {
        if (_shakeTimer > 0) {
            cameraTransform.localPosition = orignalCameraPos + Random.insideUnitSphere * updateShakeAmount;
            _shakeTimer -= Time.deltaTime;
        } else {
            _shakeTimer = 0f;
            cameraTransform.position = orignalCameraPos;
            canShake = false;
        }
    }

}
