using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    [SerializeField] float _moveSpeed = 5.0f;
    [SerializeField] float _rotationSpeed = 2.0f;
    [SerializeField] Camera _camera;

    public Camera getPlayerCamera{ get { return _camera; } }
    
    Rigidbody _rigidbody;
    Animator _animator;

    Quaternion _lastRotation;
    Quaternion _fromRotation;
    Quaternion _targetRotation;

    bool _active = true;
    public bool playerActive { set { _active = value; } }
    

    float _lerpFraction;
    
	void Start () 			
	{
        _rigidbody = GetComponent<Rigidbody>();        
        _animator = GetComponent<Animator>();

        
	}	

	void Update () 	
	{
        HandleMovement();
                                      
    }

    void HandleMovement()
    {
        // if not active set move velocity to 0
        if (!_active)
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            _animator.SetFloat("velocity", 0);
            return;
        }
       
        // get forward direction of camera without the x rotation, get move direction based on camera forward/right with input, set velocity without modefying gravity      
        Vector3 cameraForwardNoXRotation = Vector3.Cross(_camera.transform.right, Vector3.up).normalized;
        Vector3 direction = ((_camera.transform.right * Input.GetAxisRaw("Horizontal")) + (cameraForwardNoXRotation * Input.GetAxisRaw("Vertical"))).normalized;    
        _rigidbody.velocity = new Vector3(direction.x * _moveSpeed, _rigidbody.velocity.y, direction.z * _moveSpeed);

        // if we have input we will lerp our y rotation
        float moving = 0.0f; // temp for setting animation
        if (direction != Vector3.zero)
        {
            moving = 1.0f;

            // get the rotation based on our input vector 
            _targetRotation = Quaternion.LookRotation(direction, Vector3.up);          

            // if vector was not the same as last frame we need to restart slerp
            if (_targetRotation != _lastRotation)
            {
                // set that we are going to start lerping from our current rotation
                // set that next frames last rotation is based on the input direction of this frame
                // reset fraction to 0
                _fromRotation = transform.rotation;
                _lastRotation = _targetRotation;
                _lerpFraction = 0.0f;
            }
            
        }

        // add on to fraction
        _lerpFraction += Time.deltaTime * _rotationSpeed;
             
        // lerp euler angles instead of quaternions to avoid undefined behaviour on 180 degree rotations
        float yRotation = Mathf.LerpAngle(_fromRotation.eulerAngles.y, _targetRotation.eulerAngles.y, _lerpFraction);
        _rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z));

        _animator.SetFloat("velocity", moving);
        

    }

}


