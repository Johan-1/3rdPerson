using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] Transform _target;

    [SerializeField] Vector3 _cameraDistanceMinMaxDesired; 
    [SerializeField] Vector2 _cameraRotationXMinMax = new Vector2(-10, 10);            
    [SerializeField] Vector2 _mouseSensitivityXY;
    [SerializeField] Vector2 _stickSensitivityXY;

    [SerializeField] float _lerpSpeed = 0.5f;
    [SerializeField] float _zoomSpeedMouseScroll;
    [SerializeField] float _zoomSpeedController;

    float _targetCameraDistance;  
    float _lastRayHitLenght = 0.0f;
    float _lerpOutFraction = 0.0f;

    Vector3 _cameraRotation;


    bool _haveControl = true;
    public bool haveControl { set { _haveControl = value; } }

    void Awake()
    {
        _targetCameraDistance = _cameraDistanceMinMaxDesired.z;
        _lastRayHitLenght = _cameraDistanceMinMaxDesired.z;

        // set start rotation to same as player
        _cameraRotation = _target.transform.rotation.eulerAngles;               
    }

    void LateUpdate()
    {
        if (!_haveControl)
            return;

        ZoomCamera();
        MoveCamera();       
        CheckInterseption();        
    }

    void CheckInterseption()
    {

        // cast 3 rays from player towards the camera        
        Vector3[] targetToCameraFLR = new Vector3[3];

        targetToCameraFLR[0] = (transform.position + new Vector3(0, -0.5f, 0)) - _target.position; // straight from below for floor collision
        targetToCameraFLR[1] = (transform.position + transform.right * 0.35f) - _target.position;  // Left for wall collision
        targetToCameraFLR[2] = (transform.position + -transform.right * 0.35f) - _target.position; // right for wall collision

        // move the camera a little extra back from the hitpoint, this makes the camera backup smooth against the floor
        float extraDistance = 0.01f;       
        int hitCount = 0; // count how many rays that hit something
        RaycastHit hit;

        for (int i =0; i < 3; i++)
        {
            targetToCameraFLR[i].Normalize();
            if (Physics.Raycast(_target.position, targetToCameraFLR[i], out hit, _targetCameraDistance))
            {
                // set the targetDistance to the lenght of ray from origin to hitpoint
                // set the LastRayHitLenght to the same value. we will start our out lerp from this value if we dont hit anything the next frame
                // set lerp fraction to be 0 if anything is hit so lerp will restart from beginning
                _targetCameraDistance = hit.distance + extraDistance;
                _lastRayHitLenght = _targetCameraDistance;
                _lerpOutFraction = 0;

                hitCount++;
            }
        }

        // if no ray hit it is safe to start lerping back out to max distance of camera
        if (hitCount == 0)
        {
            
            _lerpOutFraction += Time.deltaTime * _lerpSpeed;
            _lerpOutFraction = Mathf.Clamp01(_lerpOutFraction);
            _targetCameraDistance = Mathf.Lerp(_lastRayHitLenght, _cameraDistanceMinMaxDesired.z, _lerpOutFraction);
        }

#if UNITY_EDITOR

        // draw rays for debuging
        Debug.DrawRay(_target.position, targetToCameraFLR[0] * _targetCameraDistance, Color.cyan);
        Debug.DrawRay(_target.position, targetToCameraFLR[1] * _targetCameraDistance, Color.cyan);
        Debug.DrawRay(_target.position, targetToCameraFLR[2] * _targetCameraDistance, Color.cyan);        
#endif
    }

    void ZoomCamera()
    {
        // only allow to zoom in/out if our camera is not colliding or is lerping from a collision
        if (_lerpOutFraction != 1.0f)
            return;
       
        // mouse scrollwheel
        _cameraDistanceMinMaxDesired.z += Input.GetAxisRaw("ZoomMouse") * _zoomSpeedMouseScroll;

        //Controller triggers
        _cameraDistanceMinMaxDesired.z += Input.GetAxisRaw("ZoomController")  * _zoomSpeedController * Time.deltaTime;
        
        // clamp value to min/max
        _cameraDistanceMinMaxDesired.z = Mathf.Clamp(_cameraDistanceMinMaxDesired.z, _cameraDistanceMinMaxDesired.x, _cameraDistanceMinMaxDesired.y);

        // set target distance to new desired distance
        _targetCameraDistance = _cameraDistanceMinMaxDesired.z;                         

    }
       

    void MoveCamera()
    {
        
        // get mouse input and add to rotation
        _cameraRotation.y += Input.GetAxisRaw("Mouse X") * _mouseSensitivityXY.x;
        _cameraRotation.x += Input.GetAxisRaw("Mouse Y") * _mouseSensitivityXY.y;

        // get right controllstick and add to rotation
        _cameraRotation.y += Input.GetAxisRaw("RightStickX") * _stickSensitivityXY.x * Time.deltaTime;
        _cameraRotation.x += Input.GetAxisRaw("RightStickY") * _stickSensitivityXY.y * Time.deltaTime;

        // clamp the x rotation of camera to avoid flipping
        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, _cameraRotationXMinMax.x, _cameraRotationXMinMax.y);

        // set the position to the position of target and add the rotation multiplied by distance to make camera orbit around player
        transform.position = _target.position + Quaternion.Euler(_cameraRotation) * new Vector3(0, 0, -_targetCameraDistance);

        // last focus forward on player
        transform.LookAt(_target.transform);   
               
    }

}
