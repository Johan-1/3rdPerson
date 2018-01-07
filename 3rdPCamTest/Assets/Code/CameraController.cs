using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] Transform _target;
    [SerializeField] Vector2 _cameraRotationXMinMax = new Vector2(-10, 10);

    [SerializeField] float _cameraDistance = 1.0f;
    [SerializeField] float _mouseSensitivityY = 2.0f;
    [SerializeField] float _mouseSensitivityX = 0.2f;

    [SerializeField] Vector2 _stickSensitivity;

    float _usingDistance;

    Vector3 _cameraRotation = new Vector3(0, 180, 0);

    void Awake()
    {
        _usingDistance = _cameraDistance;    
    }

    void LateUpdate()
    {




        

        CheckCollision();
        CheckInterseption();
        MoveCamera();



    }

    void CheckInterseption()
    {

        Vector3 targetToCamera = (transform.position + new Vector3(0, -0.5f,0)) - _target.position;
        targetToCamera.Normalize();

        float extraDistance = 0.5f;

        RaycastHit hit;

        if (Physics.Raycast(_target.position, targetToCamera, out hit, _usingDistance))
        {           
            transform.position = hit.transform.position + hit.normal * extraDistance;
            
            if(hit.distance + extraDistance < _cameraDistance)
               _usingDistance = hit.distance + extraDistance;
                        
        }


        if (!Physics.Raycast(_target.position, targetToCamera, out hit, _cameraDistance + 0.4f))
        {            
            _usingDistance = _cameraDistance;
        }


        
        
       
        Debug.DrawRay(_target.position, targetToCamera * _usingDistance, Color.cyan);
        

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1.0f);
    }

    void CheckCollision()
    {
        if (Physics.CheckSphere(transform.position, 1.0f))
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, 1.0f);


            
            print("camera colliding");
            
        }
        
    }

    void MoveCamera()
    {
        //get y and x rotations and clamp to desired values, set the local rotation of shoulder camera
        _cameraRotation.y += Input.GetAxisRaw("Mouse X") * _mouseSensitivityY;
        _cameraRotation.x += Input.GetAxisRaw("Mouse Y") * _mouseSensitivityY;

        _cameraRotation.y += Input.GetAxisRaw("RightStickX") * _stickSensitivity.x * Time.deltaTime;
        _cameraRotation.x += Input.GetAxisRaw("RightStickY") * _stickSensitivity.y * Time.deltaTime;


        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, _cameraRotationXMinMax.x, _cameraRotationXMinMax.y);

        transform.position = _target.position + Quaternion.Euler(_cameraRotation) * new Vector3(0, 0, _usingDistance);

        transform.LookAt(_target.transform);
    }

}
