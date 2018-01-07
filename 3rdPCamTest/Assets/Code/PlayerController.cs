using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerController : MonoBehaviour 
{
    Vector3 _direction;
    Vector3 _rotation;
    Rigidbody _rigidbody;
    [SerializeField] float _moveSpeed = 5.0f;


    [SerializeField] Camera _camera;
    

    
    

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
       
        

        
        
    }

    void SetVelocity()
    {

        // get forward direction based on camera
        Vector3 camToPlayer = transform.position - new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);
        camToPlayer.Normalize();

        //movement
        _direction = _camera.transform.right * Input.GetAxisRaw("Horizontal") + camToPlayer  * Input.GetAxisRaw("Vertical");
        _direction.Normalize();
        _rigidbody.velocity = new Vector3(_direction.x * _moveSpeed, _rigidbody.velocity.y, _direction.z * _moveSpeed);

        

        // animation and forward facing
        float moving = 0.0f;
        if (_direction != Vector3.zero)
        {
            moving = 1.0f;
            transform.forward = _direction;

        }            
        _animator.SetFloat("velocity", moving);

    }

    void SetRotations()
    {



    }
















}


