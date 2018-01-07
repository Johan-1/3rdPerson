using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerController : MonoBehaviour 
{
    Vector3 _direction;
    Vector3 _rotation;
    Rigidbody _rigidbody;
    [SerializeField] float _moveSpeed = 5.0f;

    [SerializeField] Camera _shoulderCamera;
    [SerializeField] Transform _cameraLookPoint;
    [SerializeField] float _cameraDistance = 1.0f;
    [SerializeField] float _mouseSensitivityY = 2.0f;
    [SerializeField] float _mouseSensitivityX = 0.2f;

    [SerializeField] Vector2 _cameraRotationYMinMax = new Vector2(-20,30);
    [SerializeField] Vector2 _cameraRotationXMinMax = new Vector2(-10,10);

    Vector3 _cameraRotation;

    Animator _animator;

	void Start () 			
	{
        _rigidbody = GetComponent<Rigidbody>();
        
        _animator = GetComponent<Animator>();
	}

	

	void Update () 	
	{
        SetVelocity();
        SetRotations();
       
        _shoulderCamera.transform.position = _cameraLookPoint.position -_shoulderCamera.transform.forward * _cameraDistance;

        _animator.SetFloat("VelocityZ", Input.GetAxisRaw("Vertical"));
        _animator.SetFloat("VelocityX", Input.GetAxisRaw("Horizontal"));
    }

    void SetVelocity()
    {
        //movement
        _direction = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
        _direction.Normalize();
        _rigidbody.velocity = new Vector3(_direction.x * _moveSpeed, _rigidbody.velocity.y, _direction.z * _moveSpeed);

    }

    void SetRotations()
    {
        //get y and x rotations and clamp to desired values, set the local rotation of shoulder camera
        _cameraRotation.y += Input.GetAxisRaw("Mouse X") * _mouseSensitivityY;
        _cameraRotation.x += Input.GetAxisRaw("Mouse Y") * _mouseSensitivityY;

        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, _cameraRotationYMinMax.x, _cameraRotationYMinMax.y);
        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, _cameraRotationXMinMax.x, _cameraRotationXMinMax.y);

        _shoulderCamera.transform.localRotation = Quaternion.Euler(_cameraRotation);

        //if camera rotation is at clamped value start rotating player body aswell
        if (_cameraRotation.y < _cameraRotationYMinMax.x + 1 || _cameraRotation.y > _cameraRotationYMinMax.y - 1)
        {
            _rotation.y += Input.GetAxisRaw("Mouse X") * _mouseSensitivityY;
            transform.rotation = Quaternion.Euler(_rotation);
        }              
    }
















}


