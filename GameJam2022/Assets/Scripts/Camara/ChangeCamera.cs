using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeCamera : MonoBehaviour
{ 
    [SerializeField]
    
    private CinemachineVirtualCamera _followCam;


    [SerializeField]

    private CinemachineVirtualCamera _staticCam;


    void OnTriggerEnter(Collider other)
    {
        _followCam.Priority = 0;
        _staticCam.Priority = 1;
    }

    void OnTriggerExit(Collider other)
    {
        _followCam.Priority = 1;
        _staticCam.Priority = 0;
    }
    
}
