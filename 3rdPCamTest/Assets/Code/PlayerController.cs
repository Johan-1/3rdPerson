using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;


public class PlayerController : MonoBehaviour 
{
        
    [SerializeField] float _moveSpeed = 5.0f;
    [SerializeField] Camera _camera;        
    
    Rigidbody _rigidbody;
    Animator _animator;

	void Start () 			
	{
        _rigidbody = GetComponent<Rigidbody>();        
        _animator = GetComponent<Animator>();
	}

	

	void Update () 	
	{
        SetVelocity();
                                      
    }

    void SetVelocity()
    {
        // get forward direction based on camera without the hight difference (dont want a forward leaning downwards/upwards)
        Vector3 camToPlayer = transform.position - new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);
        camToPlayer.Normalize();

        // get input direction based on the view from camera
        Vector3 direction = ((_camera.transform.right * Input.GetAxisRaw("Horizontal")) + (camToPlayer  * Input.GetAxisRaw("Vertical"))).normalized;
        
        // set velocity keeping gravity
        _rigidbody.velocity = new Vector3(direction.x * _moveSpeed, _rigidbody.velocity.y, direction.z * _moveSpeed);
        
        // set forward of model to direction of input
        // only set if we have input to avoid rotation snapping back when standing still
        float moving = 0.0f;
        if (direction != Vector3.zero)
        {
            moving = 1.0f;
            transform.forward = direction;
        }     
        
        _animator.SetFloat("velocity", moving);

    }

    
















}


