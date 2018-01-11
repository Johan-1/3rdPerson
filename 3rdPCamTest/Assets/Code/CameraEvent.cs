using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvent : MonoBehaviour
{
    bool triggered = false;


    [SerializeField] Transform _eventDestination;
    [SerializeField] float _panDurationInSec = 3.0f;
    [SerializeField] float _waitBeforeEventStartinSec = 0.0f;
    [SerializeField] float _waitBeforePanbackInSec = 3.0f;
    [SerializeField] float _waitBeforeStartPanInsec = 0.5f;
    [SerializeField] AnimationCurve _lerpCurve;


    [SerializeField] GameObject _objectToAnimate;

   

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 8)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            CameraController camera = player.getPlayerCamera.GetComponent<CameraController>();

            player.playerActive = false;
            camera.haveControl = false;

            StartCoroutine(DoCameraEvent(player, camera));
            
        }

    }


    IEnumerator DoCameraEvent(PlayerController player, CameraController camera)
    {

        yield return new WaitForSeconds(_waitBeforeStartPanInsec);

        Vector3 startPos = camera.transform.position;
        Quaternion startRotation = camera.transform.rotation;
        

        float fraction = 0.0f;
        float curveFraction = 0.0f;

        while (fraction < 1.0f)
        {
            curveFraction += (Time.deltaTime / _panDurationInSec);
            fraction += (Time.deltaTime / _panDurationInSec) * _lerpCurve.Evaluate(curveFraction);

            camera.transform.position = Vector3.Lerp(startPos, _eventDestination.position, fraction);
            camera.transform.rotation = Quaternion.Lerp(startRotation, _eventDestination.rotation, fraction);

            

            yield return null;
        }

        yield return new WaitForSeconds(_waitBeforeEventStartinSec);

        //start whatever event animations
        _objectToAnimate.GetComponent<Animator>().SetFloat("Speed", 1.0f);

        yield return new WaitForSeconds(_waitBeforePanbackInSec);

        fraction = 0.0f;
        curveFraction = 0.0f;

        while (fraction < 1.0f)
        {

            curveFraction += (Time.deltaTime / _panDurationInSec);
            fraction += (Time.deltaTime / _panDurationInSec) * _lerpCurve.Evaluate(curveFraction);
            
            camera.transform.rotation = Quaternion.Lerp(_eventDestination.rotation, startRotation, fraction);
            camera.transform.position = Vector3.Lerp(_eventDestination.position, startPos, fraction);
            

            yield return null;
        }


        player.playerActive = true;
        camera.haveControl = true;

    }

}
