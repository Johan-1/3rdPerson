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

        // check if is player
        if (other.gameObject.layer == 8)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            CameraController camera = player.getPlayerCamera.GetComponent<CameraController>();

            // remove controll from player
            player.playerActive = false;
            camera.haveControl = false;

            // start routine that handle the camerapan
            StartCoroutine(DoCameraEvent(player, camera));
            
        }

    }


    IEnumerator DoCameraEvent(PlayerController player, CameraController camera)
    {

        // wait before starting camera pan
        yield return new WaitForSeconds(_waitBeforeStartPanInsec);

        // get the current posoition and rotation of the camera before we start pan
        Vector3 startPos = camera.transform.position;
        Quaternion startRotation = camera.transform.rotation;
        
        // the fraction of the progress of the pan and progress of animationcurve
        float fraction = 0.0f;
        float curveFraction = 0.0f;

        while (fraction < 1.0f)
        {
            // add to animationcurve fraction
            curveFraction += (Time.deltaTime / _panDurationInSec);

            // fraction is based on desired seconds multiplied by the progress of curve
            fraction += (Time.deltaTime / _panDurationInSec) * _lerpCurve.Evaluate(curveFraction);

            camera.transform.position = Vector3.Lerp(startPos, _eventDestination.position, fraction);
            camera.transform.rotation = Quaternion.Lerp(startRotation, _eventDestination.rotation, fraction);
            
            yield return null;
        }

        // wait before starting the events
        yield return new WaitForSeconds(_waitBeforeEventStartinSec);

        //start whatever event animation
        _objectToAnimate.GetComponent<Animator>().SetFloat("Speed", 1.0f);

        // wait before we panback
        yield return new WaitForSeconds(_waitBeforePanbackInSec);

        // do the same as above but reversed
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

        // give the player back control
        player.playerActive = true;
        camera.haveControl = true;

    }

}
